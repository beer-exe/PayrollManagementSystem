import React, { useEffect, useState } from 'react';
import type { LichLamViecDto } from '../types/workSchedule.types';
import { useChiTietLich } from '../hooks/useWorkSchedule';
import './WorkScheduleManagement.css';

interface Props {
  lich: LichLamViecDto;
  onClose: (hasChanges?: boolean) => void;
}

const MONTH_NAMES = [
  'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4',
  'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8',
  'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12',
];

const getRowClass = (loaiNgay: string) => {
  if (loaiNgay === 'Ngày làm việc') return 'ws-row-work';
  if (loaiNgay === 'Nghỉ lễ') return 'ws-row-holiday';
  return 'ws-row-weekend';
};

const getBadgeClass = (loaiNgay: string) => {
  if (loaiNgay === 'Ngày làm việc') return 'ws-loai-badge work';
  if (loaiNgay === 'Nghỉ lễ') return 'ws-loai-badge holiday';
  return 'ws-loai-badge weekend';
};

const formatDate = (dateStr: string) => {
  const [y, m, d] = dateStr.split('-');
  return `${d}/${m}/${y}`;
};

export const WorkScheduleDetailModal: React.FC<Props> = ({ lich, onClose }) => {
  const [selectedThang, setSelectedThang] = useState(1);
  const [page, setPage] = useState(1);
  const { chiTiets, setChiTiets, isLoading, totalRecords, totalPages, fetch } = useChiTietLich();

  const [editingRow, setEditingRow] = useState<any>(null);
  const [editLoaiNgay, setEditLoaiNgay] = useState<string>('');
  const [hasChanges, setHasChanges] = useState(false);
  const [toastMsg, setToastMsg] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  useEffect(() => {
    fetch(lich.idLich, selectedThang, page);
  }, [lich.idLich, selectedThang, page, fetch]);

  const handleMonthChange = (m: number) => {
    setSelectedThang(m);
    setPage(1);
  };

  const countByType = (type: string) =>
    chiTiets.filter((c) => c.loaiNgay === type).length;

  const totalWork = countByType('Ngày làm việc');
  const totalWeekend = countByType('Nghỉ cuối tuần');
  const totalHoliday = countByType('Nghỉ lễ');

  const showToast = (type: 'success' | 'error', text: string) => {
    setToastMsg({ type, text });
    setTimeout(() => setToastMsg(null), 3000);
  };

  const handleSaveEdit = async () => {
    if (!editingRow || !editLoaiNgay) return;
    try {
      import('../api/workScheduleApi').then(async ({ workScheduleApi }) => {
        await workScheduleApi.updateChiTiet(editingRow.id, editLoaiNgay);
        setChiTiets(chiTiets.map(c => c.id === editingRow.id ? { ...c, loaiNgay: editLoaiNgay } : c));
        setHasChanges(true);
        setEditingRow(null);
        showToast('success', 'Cập nhật loại ngày thành công.');
      });
    } catch (err: any) {
      showToast('error', err.response?.data?.message || 'Lỗi khi cập nhật loại ngày.');
      setEditingRow(null);
    }
  };

  const handleUpdateLoaiNgay = async (id: string, newLoaiNgay: string) => {
    // Kept for backward compatibility if needed, but not used directly in table anymore.
  };

  return (
    <div className="ws-modal-overlay" onClick={(e) => e.target === e.currentTarget && onClose(hasChanges)}>
      <div className="ws-modal">
        {toastMsg && <div className={`ws-toast ws-toast--${toastMsg.type}`}>{toastMsg.text}</div>}
        {/* Header */}
        <div className="ws-modal-header">
          <div>
            <h2>📅 Lịch làm việc năm {lich.nam}</h2>
            <p style={{ margin: 0, fontSize: '0.8rem', opacity: 0.85 }}>
              Tổng: {lich.tongNgayLam} ngày làm &bull; {lich.tongNgayNghiCuoiTuan} ngày T7/CN &bull; {lich.tongNgayLe} ngày lễ
            </p>
          </div>
          <button className="ws-modal-close" onClick={() => onClose(hasChanges)} aria-label="Đóng">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.2rem', height: '1.2rem' }}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Month tabs */}
        <div className="ws-month-tabs" role="tablist">
          {MONTH_NAMES.map((name, i) => (
            <button
              key={i + 1}
              role="tab"
              aria-selected={selectedThang === i + 1}
              className={`ws-month-tab${selectedThang === i + 1 ? ' active' : ''}`}
              onClick={() => handleMonthChange(i + 1)}
            >
              {name}
            </button>
          ))}
        </div>

        {/* Month summary */}
        <div className="ws-month-summary">
          <span className="ws-month-summary-chip">
            <span style={{ width: 8, height: 8, borderRadius: '50%', background: 'var(--success-text)', display: 'inline-block' }} />
            Ngày làm: <strong>{totalWork}</strong>
          </span>
          <span className="ws-month-summary-chip">
            <span style={{ width: 8, height: 8, borderRadius: '50%', background: 'var(--primary)', display: 'inline-block' }} />
            Nghỉ cuối tuần: <strong>{totalWeekend}</strong>
          </span>
          <span className="ws-month-summary-chip">
            <span style={{ width: 8, height: 8, borderRadius: '50%', background: 'var(--warning-text)', display: 'inline-block' }} />
            Nghỉ lễ: <strong>{totalHoliday}</strong>
          </span>
          <span className="ws-month-summary-chip" style={{ marginLeft: 'auto' }}>
            Tổng: <strong>{totalRecords}</strong> ngày
          </span>
        </div>

        {/* Detail table */}
        <div className="ws-modal-body">
          {isLoading ? (
            <div className="ws-spinner" />
          ) : (
            <table className="ws-detail-table" aria-label={`Lịch làm việc tháng ${selectedThang} năm ${lich.nam}`}>
              <thead>
                <tr>
                  <th style={{ width: '3rem' }}>STT</th>
                  <th>Ngày</th>
                  <th>Thứ</th>
                  <th>Loại ngày</th>
                  <th>Tên ngày nghỉ</th>
                  <th style={{ textAlign: 'center' }}>Số giờ làm</th>
                  <th style={{ textAlign: 'center', width: '5rem' }}>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {chiTiets.length === 0 ? (
                  <tr>
                    <td colSpan={7} style={{ textAlign: 'center', padding: '3rem', color: 'var(--text-muted)' }}>
                      Không có dữ liệu
                    </td>
                  </tr>
                ) : (
                  chiTiets.map((c, idx) => (
                    <tr key={c.id} className={getRowClass(c.loaiNgay)}>
                      <td style={{ color: 'var(--text-muted)', fontSize: '0.8rem' }}>
                        {(page - 1) * 31 + idx + 1}
                      </td>
                      <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{formatDate(c.ngay)}</td>
                      <td style={{ color: 'var(--text-secondary)' }}>{c.thu}</td>
                      <td>
                        <span className={getBadgeClass(c.loaiNgay)}>{c.loaiNgay}</span>
                      </td>
                      <td style={{ color: 'var(--text-secondary)', fontSize: '0.82rem' }}>
                        {c.tenNgayNghi ?? '—'}
                      </td>
                      <td style={{ textAlign: 'center', fontWeight: 600, color: c.soGioLam > 0 ? 'var(--success-text)' : 'var(--text-muted)' }}>
                        {c.soGioLam > 0 ? `${c.soGioLam}h` : '0h'}
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        <button 
                          className="ws-btn-actions" 
                          style={{ padding: '0.35rem', background: 'var(--bg-surface)', border: '1px solid var(--border-color)', borderRadius: '6px', cursor: 'pointer', transition: 'all 0.2s' }}
                          onClick={() => {
                            setEditingRow(c);
                            setEditLoaiNgay(c.loaiNgay);
                          }}
                          title="Sửa loại ngày"
                        >
                          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem', color: 'var(--text-secondary)' }}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L6.832 19.82a4.5 4.5 0 0 1-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 0 1 1.13-1.897L16.863 4.487Zm0 0L19.5 7.125" />
                          </svg>
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          )}
        </div>

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="ws-modal-footer">
            <span className="ws-pagination-info">
              Trang {page}/{totalPages} &bull; {totalRecords} bản ghi
            </span>
            <div className="ws-pagination-btns">
              <button
                className="ws-page-btn"
                disabled={page === 1}
                onClick={() => setPage((p) => p - 1)}
                aria-label="Trang trước"
              >‹</button>
              {Array.from({ length: totalPages }, (_, i) => i + 1)
                .filter(p => Math.abs(p - page) <= 2 || p === 1 || p === totalPages)
                .map((p, i, arr) => (
                  <React.Fragment key={p}>
                    {i > 0 && arr[i - 1] !== p - 1 && <span style={{ padding: '0 0.25rem', color: 'var(--text-muted)' }}>…</span>}
                    <button
                      className={`ws-page-btn${p === page ? ' active' : ''}`}
                      onClick={() => setPage(p)}
                    >{p}</button>
                  </React.Fragment>
                ))
              }
              <button
                className="ws-page-btn"
                disabled={page === totalPages}
                onClick={() => setPage((p) => p + 1)}
                aria-label="Trang sau"
              >›</button>
            </div>
          </div>
        )}
      </div>

      {/* Edit Sub-Modal */}
      {editingRow && (
        <div className="ws-modal-overlay" style={{ zIndex: 1100, background: 'rgba(0,0,0,0.5)', animation: 'fadeIn 0.2s ease-out' }} onClick={() => setEditingRow(null)}>
          <div className="ws-modal" style={{ width: '400px', maxWidth: '90%', animation: 'slideUp 0.3s ease-out', display: 'flex', flexDirection: 'column' }} onClick={e => e.stopPropagation()}>
            <div className="ws-modal-header" style={{ padding: '1.25rem 1.5rem', background: 'linear-gradient(135deg, #7c3aed, #6d28d9)' }}>
              <h3 style={{ margin: 0, color: '#fff', fontSize: '1.1rem', fontWeight: 600 }}>Chỉnh sửa ngày {formatDate(editingRow.ngay)}</h3>
              <button 
                onClick={() => setEditingRow(null)} 
                style={{ background: 'none', border: 'none', color: '#fff', cursor: 'pointer', opacity: 0.8 }}
                title="Đóng"
              >
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.2rem', height: '1.2rem' }}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
            <div className="ws-modal-body" style={{ padding: '1.5rem' }}>
              <label style={{ display: 'block', marginBottom: '0.5rem', fontSize: '0.9rem', fontWeight: 600, color: 'var(--text-primary)' }}>
                Loại ngày
              </label>
              <select 
                className="ws-input" 
                style={{ width: '100%', padding: '0.75rem', borderRadius: '8px', border: '1px solid var(--border-color)', outline: 'none' }}
                value={editLoaiNgay}
                onChange={(e) => setEditLoaiNgay(e.target.value)}
              >
                <option value="Ngày làm việc">Ngày làm việc</option>
                <option value="Nghỉ cuối tuần">Nghỉ cuối tuần</option>
                <option value="Nghỉ lễ">Nghỉ lễ</option>
              </select>
            </div>
            <div style={{ padding: '1rem 1.5rem', borderTop: '1px solid var(--border-color)', display: 'flex', justifyContent: 'flex-end', gap: '0.75rem' }}>
              <button 
                onClick={() => setEditingRow(null)}
                style={{ padding: '0.5rem 1rem', border: '1px solid var(--border-color)', borderRadius: '6px', background: 'var(--bg-surface)', cursor: 'pointer', fontWeight: 600, color: 'var(--text-secondary)' }}
              >
                Hủy
              </button>
              <button 
                onClick={handleSaveEdit}
                style={{ padding: '0.5rem 1rem', border: 'none', borderRadius: '6px', background: 'linear-gradient(135deg, #7c3aed, #6d28d9)', cursor: 'pointer', fontWeight: 600, color: '#fff', boxShadow: '0 2px 4px rgba(124, 58, 237, 0.25)' }}
              >
                Lưu thay đổi
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
