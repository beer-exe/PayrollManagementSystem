import React, { useState, useEffect, useCallback, useRef } from 'react';
import { useDonNghi } from '../hooks/useDonNghi';
import { departmentApi } from '../../departments/api/departmentApi';
import { useAuthStore } from '@/store/useAuthStore';
import type { DonNghiDto, NgayPhepDto, UpdateNgayPhepRequest } from '../types/donNghi.types';
import type { DepartmentDto } from '../../departments/types/department.types';
import { DonNghiFormModal } from './DonNghiFormModal';
import './DonNghiManagement.css';

const TRANG_THAI_COLOR: Record<string, string> = {
  'Chờ duyệt': 'dn-status dn-status--pending',
  'Đã duyệt': 'dn-status dn-status--approved',
  'Từ chối': 'dn-status dn-status--rejected',
};

const LOAI_NGHI_COLOR: Record<string, string> = {
  'Nghỉ phép năm': 'dn-badge dn-badge--blue',
  'Nghỉ không lương': 'dn-badge dn-badge--gray',
  'Nghỉ ốm đau': 'dn-badge dn-badge--orange',
  'Nghỉ thai sản': 'dn-badge dn-badge--purple',
  'Nghỉ theo chế độ': 'dn-badge dn-badge--teal',
};

const now = new Date();
const PAGE_SIZE = 10;

export const DonNghiManagement: React.FC = () => {
  const { user } = useAuthStore();
  const userRole = user?.role || '';
  const isHR = userRole === 'HR';

  const [activeTab, setActiveTab] = useState<'don-nghi' | 'ngay-phep'>('don-nghi');
  const [thang, setThang] = useState(now.getMonth() + 1);
  const [nam, setNam] = useState(now.getFullYear());
  const [filterTrangThai, setFilterTrangThai] = useState('');
  const [searchText, setSearchText] = useState('');

  // Phòng ban filter
  const [departments, setDepartments] = useState<DepartmentDto[]>([]);
  const [idPhongBan, setIdPhongBan] = useState('');
  const [pbSearchTerm, setPbSearchTerm] = useState('');
  const [isPbOpen, setIsPbOpen] = useState(false);
  const pbRef = useRef<HTMLDivElement>(null);

  // Pagination
  const [donNghiPage, setDonNghiPage] = useState(1);
  const [ngayPhepPage, setNgayPhepPage] = useState(1);

  // Modals
  const [showFormModal, setShowFormModal] = useState(false);
  const [openMenuId, setOpenMenuId] = useState<string | null>(null);
  const [tuChoiId, setTuChoiId] = useState<string | null>(null);
  const [tuChoiLyDo, setTuChoiLyDo] = useState('');
  const [ngayPhepEdit, setNgayPhepEdit] = useState<NgayPhepDto | null>(null);
  const [ngayPhepForm, setNgayPhepForm] = useState({ cccd: '', nam: now.getFullYear(), tong: 12 });
  const [showNgayPhepModal, setShowNgayPhepModal] = useState(false);

  const [toastMsg, setToastMsg] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  const { list, ngayPhepList, loading, error, fetchList, fetchNgayPhep, createDonNghi, duyetDonNghi, tuChoiDonNghi, deleteDonNghi, updateNgayPhep } = useDonNghi();

  useEffect(() => {
    departmentApi.getDepartments()
      .then(res => { if (res.data) setDepartments(res.data); })
      .catch(console.error);

    const handleClickOutside = (e: MouseEvent) => {
      if (pbRef.current && !pbRef.current.contains(e.target as Node)) setIsPbOpen(false);
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const filteredDepts = departments.filter(d => d.tenPb.toLowerCase().includes(pbSearchTerm.toLowerCase()));

  const loadData = useCallback(() => {
    if (activeTab === 'don-nghi') {
      fetchList({ thang, nam, trangThai: filterTrangThai || undefined, idPhongBan: idPhongBan || undefined });
    } else {
      fetchNgayPhep(nam, idPhongBan || undefined);
    }
  }, [activeTab, thang, nam, filterTrangThai, idPhongBan, fetchList, fetchNgayPhep]);

  useEffect(() => { loadData(); }, [loadData]);

  useEffect(() => { setDonNghiPage(1); setNgayPhepPage(1); }, [thang, nam, filterTrangThai, idPhongBan, activeTab]);

  const showToast = (type: 'success' | 'error', text: string) => {
    setToastMsg({ type, text });
    setTimeout(() => setToastMsg(null), 3500);
  };

  // Filtered list (client-side search on name)
  const filteredList = list.filter(r =>
    !searchText || r.hoTenNhanVien.toLowerCase().includes(searchText.toLowerCase()) || r.cccdNhanVien.includes(searchText)
  );

  const totalDonNghiPages = Math.max(1, Math.ceil(filteredList.length / PAGE_SIZE));
  const currentDonNghiList = filteredList.slice((donNghiPage - 1) * PAGE_SIZE, donNghiPage * PAGE_SIZE);
  const totalNgayPhepPages = Math.max(1, Math.ceil(ngayPhepList.length / PAGE_SIZE));
  const currentNgayPhepList = ngayPhepList.slice((ngayPhepPage - 1) * PAGE_SIZE, ngayPhepPage * PAGE_SIZE);

  const handleCreate = async (data: Parameters<typeof createDonNghi>[0]) => {
    const err = await createDonNghi(data);
    if (err) { showToast('error', err); return false; }
    showToast('success', 'Tạo đơn nghỉ thành công!');
    loadData();
    return true;
  };

  const handleDuyet = async (id: string) => {
    const err = await duyetDonNghi(id);
    if (err) showToast('error', err);
    else { showToast('success', 'Đã duyệt đơn nghỉ!'); loadData(); }
    setOpenMenuId(null);
  };

  const handleTuChoi = async () => {
    if (!tuChoiId || !tuChoiLyDo.trim()) return;
    const err = await tuChoiDonNghi(tuChoiId, { lyDoTuChoi: tuChoiLyDo });
    if (err) showToast('error', err);
    else { showToast('success', 'Đã từ chối đơn nghỉ!'); loadData(); }
    setTuChoiId(null); setTuChoiLyDo('');
  };

  const handleDelete = async (id: string, name: string) => {
    if (!confirm(`Xác nhận xóa đơn nghỉ của "${name}"?`)) return;
    const err = await deleteDonNghi(id);
    if (err) showToast('error', err);
    else { showToast('success', 'Xóa đơn thành công!'); loadData(); }
    setOpenMenuId(null);
  };

  const handleUpdateNgayPhep = async () => {
    const req: UpdateNgayPhepRequest = {
      cccdNhanVien: ngayPhepForm.cccd,
      nam: ngayPhepForm.nam,
      tongNgayPhep: ngayPhepForm.tong,
    };
    const err = await updateNgayPhep(req);
    if (err) showToast('error', err);
    else { showToast('success', 'Cập nhật quota phép thành công!'); setShowNgayPhepModal(false); loadData(); }
  };

  const openNgayPhepModal = (row?: NgayPhepDto) => {
    if (row) {
      setNgayPhepForm({ cccd: row.cccdNhanVien, nam: row.nam, tong: row.tongNgayPhep });
      setNgayPhepEdit(row);
    } else {
      setNgayPhepForm({ cccd: '', nam: now.getFullYear(), tong: 12 });
      setNgayPhepEdit(null);
    }
    setShowNgayPhepModal(true);
  };

  return (
    <div className="dn-page">
      {/* TOAST */}
      {toastMsg && <div className={`dn-toast dn-toast--${toastMsg.type}`}>{toastMsg.text}</div>}

      {/* HEADER */}
      <div className="dn-header">
        <div className="dn-header__left">
          <h1 className="dn-title">Quản lý Đơn Xin Nghỉ</h1>
          <p className="dn-subtitle">Quản lý đơn nghỉ và quota phép năm của nhân viên</p>
        </div>
        {isHR && (
          <div className="dn-header__actions">
            {activeTab === 'don-nghi' && (
              <button className="dn-btn dn-btn--primary" onClick={() => setShowFormModal(true)}>
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" width={16} height={16}>
                  <path d="M10.75 4.75a.75.75 0 0 0-1.5 0v4.5h-4.5a.75.75 0 0 0 0 1.5h4.5v4.5a.75.75 0 0 0 1.5 0v-4.5h4.5a.75.75 0 0 0 0-1.5h-4.5v-4.5Z" />
                </svg>
                Tạo đơn nghỉ
              </button>
            )}
            {activeTab === 'ngay-phep' && (
              <button className="dn-btn dn-btn--primary" onClick={() => openNgayPhepModal()}>
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" width={16} height={16}>
                  <path d="M10.75 4.75a.75.75 0 0 0-1.5 0v4.5h-4.5a.75.75 0 0 0 0 1.5h4.5v4.5a.75.75 0 0 0 1.5 0v-4.5h4.5a.75.75 0 0 0 0-1.5h-4.5v-4.5Z" />
                </svg>
                Cấu hình phép
              </button>
            )}
          </div>
        )}
      </div>

      {/* FILTER BAR */}
      <div className="dn-filter-bar">
        <div className="dn-filter-group">
          <label className="dn-filter-label">Tháng</label>
          <select className="dn-select" value={thang} onChange={e => setThang(+e.target.value)}>
            {Array.from({ length: 12 }, (_, i) => i + 1).map(m => (
              <option key={m} value={m}>Tháng {m}</option>
            ))}
          </select>
        </div>
        <div className="dn-filter-group">
          <label className="dn-filter-label">Năm</label>
          <select className="dn-select" value={nam} onChange={e => setNam(+e.target.value)}>
            {[2024, 2025, 2026, 2027].map(y => <option key={y} value={y}>{y}</option>)}
          </select>
        </div>
        <div className="dn-filter-group" style={{ flex: 1, minWidth: 200 }}>
          <label className="dn-filter-label">Phòng ban</label>
          <div className="dn-dropdown-select-wrap" ref={pbRef}>
            <input
              className="dn-input"
              style={{ width: '100%' }}
              placeholder="-- Tất cả --"
              value={pbSearchTerm}
              onChange={e => { setPbSearchTerm(e.target.value); setIdPhongBan(''); setIsPbOpen(true); }}
              onFocus={() => { setPbSearchTerm(''); setIdPhongBan(''); setIsPbOpen(true); }}
              autoComplete="off"
            />
            {isPbOpen && (
              <ul className="dn-dropdown-select-list">
                <li className={!idPhongBan ? 'selected' : ''} onClick={() => { setIdPhongBan(''); setPbSearchTerm(''); setIsPbOpen(false); }}>-- Tất cả --</li>
                {filteredDepts.length > 0
                  ? filteredDepts.map(d => (
                    <li key={d.idPb} className={idPhongBan === d.idPb ? 'selected' : ''}
                      onClick={() => { setIdPhongBan(d.idPb); setPbSearchTerm(d.tenPb); setIsPbOpen(false); }}>
                      {d.tenPb}
                    </li>
                  ))
                  : <li className="dn-empty-option">Không tìm thấy</li>}
              </ul>
            )}
          </div>
        </div>
        {activeTab === 'don-nghi' && (
          <>
            <div className="dn-filter-group">
              <label className="dn-filter-label">Trạng thái</label>
              <select className="dn-select" value={filterTrangThai} onChange={e => setFilterTrangThai(e.target.value)}>
                <option value="">-- Tất cả --</option>
                <option value="CHO_DUYET">Chờ duyệt</option>
                <option value="DA_DUYET">Đã duyệt</option>
                <option value="TU_CHOI">Từ chối</option>
              </select>
            </div>
            <div className="dn-filter-group">
              <label className="dn-filter-label">Tìm kiếm</label>
              <input className="dn-input" placeholder="Tên, CCCD..." value={searchText} onChange={e => setSearchText(e.target.value)} />
            </div>
          </>
        )}
      </div>

      {/* TABS */}
      <div className="dn-tabs">
        <button className={`dn-tab ${activeTab === 'don-nghi' ? 'active' : ''}`} onClick={() => setActiveTab('don-nghi')}>
          Danh sách đơn nghỉ
        </button>
        <button className={`dn-tab ${activeTab === 'ngay-phep' ? 'active' : ''}`} onClick={() => setActiveTab('ngay-phep')}>
          Quota phép năm
        </button>
      </div>

      {/* CONTENT */}
      {loading ? (
        <div className="dn-loading"><div className="dn-spinner" /><span>Đang tải...</span></div>
      ) : error ? (
        <div className="dn-error">{error}</div>
      ) : activeTab === 'don-nghi' ? (
        /* BẢNG ĐƠN NGHỈ */
        <div className="dn-table-wrap">
          <table className="dn-table">
            <thead>
              <tr>
                <th>Nhân viên</th>
                <th>Phòng ban</th>
                <th>Loại nghỉ</th>
                <th>Từ ngày</th>
                <th>Đến ngày</th>
                <th>Số ngày</th>
                <th>Lý do</th>
                <th>Trạng thái</th>
                <th>Người duyệt</th>
                {isHR && <th></th>}
              </tr>
            </thead>
            <tbody>
              {currentDonNghiList.length === 0 ? (
                <tr><td colSpan={isHR ? 10 : 9} className="dn-empty">Không có đơn nghỉ nào trong kỳ này</td></tr>
              ) : currentDonNghiList.map((row: DonNghiDto) => (
                <tr key={row.id}>
                  <td>
                    <div className="dn-nv-name">{row.hoTenNhanVien}</div>
                    <div className="dn-nv-cccd">{row.cccdNhanVien}</div>
                  </td>
                  <td>{row.tenPhongBan ?? '—'}</td>
                  <td><span className={LOAI_NGHI_COLOR[row.loaiNghi] ?? 'dn-badge'}>{row.loaiNghi}</span></td>
                  <td className="dn-date">{new Date(row.ngayBatDau + 'T00:00:00').toLocaleDateString('vi-VN')}</td>
                  <td className="dn-date">{new Date(row.ngayKetThuc + 'T00:00:00').toLocaleDateString('vi-VN')}</td>
                  <td className="dn-num">{row.soNgayNghi}</td>
                  <td className="dn-note" title={row.lyDo}>{row.lyDo}</td>
                  <td>
                    <span className={TRANG_THAI_COLOR[row.trangThai] ?? 'dn-status'}>{row.trangThai}</span>
                    {row.lyDoTuChoi && <div className="dn-tu-choi-note" title={row.lyDoTuChoi}>↳ {row.lyDoTuChoi}</div>}
                  </td>
                  <td>{row.hoTenNguoiDuyet ?? '—'}</td>
                  {isHR && (
                    <td className="dn-actions-cell">
                      <div className="dn-dropdown-wrap">
                        <button className="dn-actions-btn" onClick={() => setOpenMenuId(openMenuId === row.id ? null : row.id)}>•••</button>
                        {openMenuId === row.id && (
                          <div className="dn-dropdown">
                            {row.trangThai === 'Chờ duyệt' && (
                              <>
                                <button onClick={() => handleDuyet(row.id)}>✅ Duyệt</button>
                                <button onClick={() => { setTuChoiId(row.id); setOpenMenuId(null); }}>❌ Từ chối</button>
                                <button className="dn-dropdown__danger" onClick={() => handleDelete(row.id, row.hoTenNhanVien)}>🗑️ Xóa</button>
                              </>
                            )}
                            {row.trangThai !== 'Chờ duyệt' && (
                              <button disabled className="dn-dropdown__disabled">Không có hành động</button>
                            )}
                          </div>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
          {filteredList.length > 0 && (
            <div className="dn-pagination">
              <button className="dn-page-btn" disabled={donNghiPage === 1} onClick={() => setDonNghiPage(p => p - 1)}>Trước</button>
              <span className="dn-page-info">{donNghiPage}/{totalDonNghiPages}</span>
              <button className="dn-page-btn" disabled={donNghiPage === totalDonNghiPages} onClick={() => setDonNghiPage(p => p + 1)}>Sau</button>
            </div>
          )}
        </div>
      ) : (
        /* BẢNG QUOTA PHÉP */
        <div className="dn-table-wrap">
          <table className="dn-table">
            <thead>
              <tr>
                <th>Nhân viên</th>
                <th>Phòng ban</th>
                <th>Năm</th>
                <th>Tổng phép</th>
                <th>Đã dùng</th>
                <th>Còn lại</th>
                {isHR && <th></th>}
              </tr>
            </thead>
            <tbody>
              {currentNgayPhepList.length === 0 ? (
                <tr><td colSpan={isHR ? 7 : 6} className="dn-empty">Chưa có cấu hình quota phép cho năm {nam}</td></tr>
              ) : currentNgayPhepList.map((row: NgayPhepDto) => (
                <tr key={row.id}>
                  <td>
                    <div className="dn-nv-name">{row.hoTenNhanVien}</div>
                    <div className="dn-nv-cccd">{row.cccdNhanVien}</div>
                  </td>
                  <td>{row.tenPhongBan ?? '—'}</td>
                  <td className="dn-num">{row.nam}</td>
                  <td className="dn-num">{row.tongNgayPhep}</td>
                  <td className="dn-num">{row.daSuDung}</td>
                  <td className="dn-num">
                    <span className={row.conLai < 0 ? 'dn-num--danger' : row.conLai <= 2 ? 'dn-num--warn' : 'dn-num--ok'}>
                      {row.conLai}
                    </span>
                  </td>
                  {isHR && (
                    <td className="dn-actions-cell">
                      <button className="dn-btn dn-btn--sm dn-btn--outline" onClick={() => openNgayPhepModal(row)}>
                        ✏️ Sửa
                      </button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
          {ngayPhepList.length > 0 && (
            <div className="dn-pagination">
              <button className="dn-page-btn" disabled={ngayPhepPage === 1} onClick={() => setNgayPhepPage(p => p - 1)}>Trước</button>
              <span className="dn-page-info">{ngayPhepPage}/{totalNgayPhepPages}</span>
              <button className="dn-page-btn" disabled={ngayPhepPage === totalNgayPhepPages} onClick={() => setNgayPhepPage(p => p + 1)}>Sau</button>
            </div>
          )}
        </div>
      )}

      {/* MODAL: Tạo đơn nghỉ */}
      {showFormModal && (
        <DonNghiFormModal
          onClose={() => setShowFormModal(false)}
          onCreate={handleCreate}
        />
      )}

      {/* MODAL: Từ chối đơn */}
      {tuChoiId && (
        <div className="dn-modal-overlay">
          <div className="dn-modal dn-modal--sm">
            <div className="dn-modal-header">
              <h2>Từ chối đơn nghỉ</h2>
              <button className="dn-modal-close" onClick={() => { setTuChoiId(null); setTuChoiLyDo(''); }}>✕</button>
            </div>
            <div className="dn-modal-body">
              <label className="dn-label">Lý do từ chối <span className="dn-required">*</span></label>
              <textarea
                className="dn-textarea"
                rows={4}
                placeholder="Nhập lý do từ chối..."
                value={tuChoiLyDo}
                onChange={e => setTuChoiLyDo(e.target.value)}
              />
            </div>
            <div className="dn-modal-footer">
              <button className="dn-btn dn-btn--outline" onClick={() => { setTuChoiId(null); setTuChoiLyDo(''); }}>Hủy</button>
              <button className="dn-btn dn-btn--danger" onClick={handleTuChoi} disabled={!tuChoiLyDo.trim()}>Xác nhận từ chối</button>
            </div>
          </div>
        </div>
      )}

      {/* MODAL: Cấu hình quota phép */}
      {showNgayPhepModal && (
        <div className="dn-modal-overlay">
          <div className="dn-modal dn-modal--sm">
            <div className="dn-modal-header">
              <h2>{ngayPhepEdit ? 'Cập nhật quota phép' : 'Tạo quota phép'}</h2>
              <button className="dn-modal-close" onClick={() => setShowNgayPhepModal(false)}>✕</button>
            </div>
            <div className="dn-modal-body">
              <div className="dn-form-row">
                <label className="dn-label">CCCD Nhân viên <span className="dn-required">*</span></label>
                <input
                  className="dn-input"
                  placeholder="Nhập CCCD..."
                  value={ngayPhepForm.cccd}
                  onChange={e => setNgayPhepForm(f => ({ ...f, cccd: e.target.value }))}
                  disabled={!!ngayPhepEdit}
                />
              </div>
              <div className="dn-form-row">
                <label className="dn-label">Năm</label>
                <select className="dn-select" value={ngayPhepForm.nam} onChange={e => setNgayPhepForm(f => ({ ...f, nam: +e.target.value }))} disabled={!!ngayPhepEdit}>
                  {[2024, 2025, 2026, 2027].map(y => <option key={y} value={y}>{y}</option>)}
                </select>
              </div>
              <div className="dn-form-row">
                <label className="dn-label">Tổng số ngày phép <span className="dn-required">*</span></label>
                <input
                  type="number" min={0} step={0.5}
                  className="dn-input"
                  value={ngayPhepForm.tong}
                  onChange={e => setNgayPhepForm(f => ({ ...f, tong: +e.target.value }))}
                />
                <small className="dn-hint">Mặc định theo luật: 12 ngày/năm. HR có thể điều chỉnh theo hợp đồng.</small>
              </div>
            </div>
            <div className="dn-modal-footer">
              <button className="dn-btn dn-btn--outline" onClick={() => setShowNgayPhepModal(false)}>Hủy</button>
              <button className="dn-btn dn-btn--primary" onClick={handleUpdateNgayPhep}>Lưu</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
