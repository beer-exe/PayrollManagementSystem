import React, { useState, useEffect, useCallback } from 'react';
import { profileApi } from '@/features/profile/api/profileApi';
import { useMyDonNghi } from '../hooks/useMyDonNghi';
import { LOAI_NGHI_OPTIONS } from '../types/donNghi.types';
import type { DonNghiDto } from '../types/donNghi.types';
import { donNghiApi } from '../api/donNghiApi';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { SortDirection } from '../../../hooks/useDataTable';
import { Toast } from '../../../components/Toast/Toast';
import './MyDonNghiPortal.css';

const now = new Date();
const PAGE_SIZE = 10;

const TRANG_THAI_COLOR: Record<string, string> = {
  'Chờ duyệt': 'mdp-status mdp-status--pending',
  'Đã duyệt':  'mdp-status mdp-status--approved',
  'Từ chối':   'mdp-status mdp-status--rejected',
};
const LOAI_NGHI_COLOR: Record<string, string> = {
  'Nghỉ phép năm':    'mdp-badge mdp-badge--blue',
  'Nghỉ không lương': 'mdp-badge mdp-badge--gray',
  'Nghỉ ốm đau':      'mdp-badge mdp-badge--orange',
  'Nghỉ thai sản':    'mdp-badge mdp-badge--purple',
  'Nghỉ theo chế độ': 'mdp-badge mdp-badge--teal',
};

interface FormState {
  loaiNghi: string;
  ngayBatDau: string;
  ngayKetThuc: string;
  soNgayNghi: number;
  lyDo: string;
  taiLieuDinhKem: string;
}
const initialForm: FormState = {
  loaiNghi: 'NGHI_PHEP_NAM',
  ngayBatDau: '',
  ngayKetThuc: '',
  soNgayNghi: 1,
  lyDo: '',
  taiLieuDinhKem: '',
};

export const MyDonNghiPortal: React.FC = () => {
  const [thang, setThang]               = useState(now.getMonth() + 1);
  const [nam, setNam]                   = useState(now.getFullYear());
  const [filterTrangThai, setFilterTrangThai] = useState('');
  const [page, setPage]                 = useState(1);
  const [showModal, setShowModal]       = useState(false);
  const [form, setForm]                 = useState<FormState>(initialForm);
  const [formErrors, setFormErrors]     = useState<Record<string, string>>({});
  const [myCccd, setMyCccd]             = useState('');
  const [myName, setMyName]             = useState('');
  const [validYears, setValidYears] = useState<number[]>([]);
  const [sortKey, setSortKey] = useState<string>('ngayBatDau');
  const [sortDirection, setSortDirection] = useState<SortDirection>('desc');

  const { list, ngayPhep, loading, fetchList, fetchNgayPhep, createDonNghi, deleteDonNghi, toast, setToast, showToast } = useMyDonNghi();

  useEffect(() => {
    import('../../workSchedule/api/workScheduleApi').then(({ workScheduleApi }) => {
      workScheduleApi.getAll().then(res => {
        if (res.data) setValidYears(Array.from(new Set(res.data.map((w: { nam: number }) => w.nam))));
      }).catch(console.error);
    });
  }, []);

  // Fetch my profile for CCCD / name display
  useEffect(() => {
    profileApi.getMyProfile().then(res => {
      if (res.succeeded && res.data) {
        setMyCccd(res.data.cccd);
        setMyName(res.data.hoTen);
      }
    }).catch(() => {});
  }, []);

  const loadData = useCallback(() => {
    fetchList({ thang, nam, trangThai: filterTrangThai || undefined });
    fetchNgayPhep(nam);
  }, [thang, nam, filterTrangThai, fetchList, fetchNgayPhep]);

  useEffect(() => { loadData(); }, [loadData]);
  useEffect(() => { setPage(1); }, [thang, nam, filterTrangThai]);


  // Auto compute soNgayNghi when dates change
  const handleFormChange = async (field: 'ngayBatDau' | 'ngayKetThuc' | 'loaiNghi', value: string) => {
    const next = { ...form, [field]: value };
    if (next.ngayBatDau && next.ngayKetThuc) {
      const start = new Date(next.ngayBatDau);
      const end   = new Date(next.ngayKetThuc);
      const diffMs = end.getTime() - start.getTime();
      if (diffMs >= 0) {
        try {
          const res = await donNghiApi.calculateDays(next.ngayBatDau, next.ngayKetThuc, next.loaiNghi);
          if (res.succeeded) {
            next.soNgayNghi = res.data;
          } else {
             next.soNgayNghi = Math.round(diffMs / (1000 * 60 * 60 * 24)) + 1;
          }
        } catch {
          next.soNgayNghi = Math.round(diffMs / (1000 * 60 * 60 * 24)) + 1;
        }
      }
    }
    setForm(next);
  };

  const validateForm = (): boolean => {
    const errs: Record<string, string> = {};
    if (!form.ngayBatDau) errs.ngayBatDau = 'Vui lòng chọn ngày bắt đầu.';
    if (!form.ngayKetThuc) errs.ngayKetThuc = 'Vui lòng chọn ngày kết thúc.';
    if (form.ngayBatDau && form.ngayKetThuc && form.ngayKetThuc < form.ngayBatDau)
      errs.ngayKetThuc = 'Ngày kết thúc phải >= ngày bắt đầu.';

    if (form.ngayBatDau && form.ngayKetThuc) {
      const startYear = new Date(form.ngayBatDau).getFullYear();
      const endYear = new Date(form.ngayKetThuc).getFullYear();
      if (startYear !== endYear) {
        errs.ngayKetThuc = 'Ngày bắt đầu và ngày kết thúc phải cùng một năm.';
      } else if (!validYears.includes(startYear)) {
        errs.ngayKetThuc = `Chưa có lịch làm việc cho năm ${startYear}.`;
      }
    }

    if (form.soNgayNghi <= 0) {
      errs.soNgayNghi = 'Số ngày nghỉ phải lớn hơn 0.';
    } else if (form.ngayBatDau && form.ngayKetThuc) {
      const start = new Date(form.ngayBatDau);
      const end = new Date(form.ngayKetThuc);
      const diffMs = end.getTime() - start.getTime();
      if (diffMs >= 0) {
        const maxDays = Math.round(diffMs / (1000 * 60 * 60 * 24)) + 1;
        if (form.soNgayNghi > maxDays) {
          errs.soNgayNghi = `Số ngày nghỉ không được vượt quá ${maxDays} ngày.`;
        }
      }
    }

    if (!form.lyDo.trim()) errs.lyDo = 'Vui lòng nhập lý do.';
    setFormErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async () => {
    if (!validateForm()) return;
    const err = await createDonNghi({
      loaiNghi: form.loaiNghi,
      ngayBatDau: form.ngayBatDau,
      ngayKetThuc: form.ngayKetThuc,
      soNgayNghi: form.soNgayNghi,
      lyDo: form.lyDo,
      taiLieuDinhKem: form.taiLieuDinhKem || undefined,
    });
    if (err) { showToast('error', err); return; }
    showToast('success', 'Nộp đơn xin nghỉ thành công!');
    setShowModal(false);
    setForm(initialForm);
    setFormErrors({});
    loadData();
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Xác nhận hủy đơn nghỉ này?')) return;
    const err = await deleteDonNghi(id);
    if (err) showToast('error', err);
    else { showToast('success', 'Đã hủy đơn nghỉ.'); loadData(); }
  };

  const handleSort = (key: string) => {
    if (sortKey === key) {
      if (sortDirection === 'asc') setSortDirection('desc');
      else if (sortDirection === 'desc') { setSortKey(''); setSortDirection('asc'); }
    } else {
      setSortKey(key);
      setSortDirection('asc');
    }
  };

  const sortedList = React.useMemo(() => {
    if (!sortKey) return list;
    return [...list].sort((a: DonNghiDto, b: DonNghiDto) => {
      const aVal = (a as any)[sortKey] ?? '';
      const bVal = (b as any)[sortKey] ?? '';
      if (aVal < bVal) return sortDirection === 'asc' ? -1 : 1;
      if (aVal > bVal) return sortDirection === 'asc' ? 1 : -1;
      return 0;
    });
  }, [list, sortKey, sortDirection]);

  const totalPages = Math.max(1, Math.ceil(sortedList.length / PAGE_SIZE));
  const currentList = sortedList.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  // Summary counts
  const pendingCount  = list.filter(d => d.trangThai === 'Chờ duyệt').length;

  return (
    <div className="mdp-page">


      {/* HEADER */}
      <div className="mdp-header">
        <div>
          <h1 className="mdp-title">Đề xuất nghỉ phép</h1>
          <p className="mdp-subtitle">
            {myName ? `${myName} — CCCD: ${myCccd}` : 'Quản lý đơn xin nghỉ của bản thân'}
          </p>
        </div>
        <button id="mdp-btn-create" className="mdp-btn mdp-btn--primary" onClick={() => setShowModal(true)}>
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" width={16} height={16}>
            <path d="M10.75 4.75a.75.75 0 0 0-1.5 0v4.5h-4.5a.75.75 0 0 0 0 1.5h4.5v4.5a.75.75 0 0 0 1.5 0v-4.5h4.5a.75.75 0 0 0 0-1.5h-4.5v-4.5Z" />
          </svg>
          Nộp đơn mới
        </button>
      </div>

      {/* SUMMARY CARDS */}
      <div className="mdp-cards">
        {ngayPhep ? (
          <>
            <div className="mdp-card">
              <span className="mdp-card__label">Tổng phép năm</span>
              <span className="mdp-card__value">{ngayPhep.tongNgayPhep}</span>
              <span className="mdp-card__sub">ngày / năm {nam}</span>
            </div>
            <div className="mdp-card">
              <span className="mdp-card__label">Đã sử dụng</span>
              <span className="mdp-card__value mdp-card__value--gray">{ngayPhep.daSuDung}</span>
              <span className="mdp-card__sub">ngày</span>
            </div>
            <div className="mdp-card">
              <span className="mdp-card__label">Còn lại</span>
              <span className={`mdp-card__value ${
                ngayPhep.conLai < 0 ? 'mdp-card__value--danger' : ngayPhep.conLai <= 2 ? 'mdp-card__value--warn' : ''
              }`}>
                {ngayPhep.conLai}
              </span>
              <span className="mdp-card__sub">ngày phép</span>
            </div>
          </>
        ) : (
          <div className="mdp-card mdp-card--empty-phep">
            <span className="mdp-card__value mdp-card__value--gray" style={{ fontSize: '15px', fontWeight: 500 }}>
              Nhân viên chưa được cấu hình ngày phép cho năm {nam}
            </span>
          </div>
        )}
        <div className="mdp-card">
          <span className="mdp-card__label">Chờ duyệt</span>
          <span className="mdp-card__value mdp-card__value--warn">{pendingCount}</span>
          <span className="mdp-card__sub">đơn đang chờ</span>
        </div>
      </div>

      {/* FILTER BAR */}
      <div className="mdp-filter-bar">
        <div className="mdp-filter-group">
          <label className="mdp-filter-label">Tháng</label>
          <select id="mdp-filter-thang" className="mdp-select" value={thang} onChange={e => setThang(+e.target.value)}>
            {Array.from({ length: 12 }, (_, i) => i + 1).map(m => (
              <option key={m} value={m}>Tháng {m}</option>
            ))}
          </select>
        </div>
        <div className="mdp-filter-group">
          <label className="mdp-filter-label">Năm</label>
          <select id="mdp-filter-nam" className="mdp-select" value={nam} onChange={e => setNam(+e.target.value)} disabled={validYears.length === 0}>
            {validYears.length > 0 
              ? validYears.map(y => <option key={y} value={y}>{y}</option>) 
              : <option value={new Date().getFullYear()}>Chưa có lịch làm việc nào được tạo.</option>}
          </select>
        </div>
        <div className="mdp-filter-group">
          <label className="mdp-filter-label">Trạng thái</label>
          <select id="mdp-filter-trangthai" className="mdp-select" value={filterTrangThai} onChange={e => setFilterTrangThai(e.target.value)}>
            <option value="">-- Tất cả --</option>
            <option value="CHO_DUYET">Chờ duyệt</option>
            <option value="DA_DUYET">Đã duyệt</option>
            <option value="TU_CHOI">Từ chối</option>
          </select>
        </div>
      </div>

      {/* TABLE */}
      {loading ? (
        <div className="mdp-loading"><div className="mdp-spinner" /><span>Đang tải...</span></div>
      ) : (
        <div className="mdp-table-wrap">
          <table className="mdp-table">
            <thead>
              <tr>
                <SortableHeader label="Loại nghỉ" sortKey="loaiNghi" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Từ ngày" sortKey="ngayBatDau" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Đến ngày" sortKey="ngayKetThuc" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Số ngày" sortKey="soNgayNghi" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <th>Lý do</th>
                <SortableHeader label="Trạng thái" sortKey="trangThai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Người duyệt" sortKey="hoTenNguoiDuyet" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <th></th>
              </tr>
            </thead>
            <tbody>
              {currentList.length === 0 ? (
                <tr><td colSpan={8} className="mdp-empty">Không có đơn nghỉ nào trong kỳ này</td></tr>
              ) : currentList.map((row: DonNghiDto) => (
                <tr key={row.id}>
                  <td><span className={LOAI_NGHI_COLOR[row.loaiNghi] ?? 'mdp-badge'}>{row.loaiNghi}</span></td>
                  <td className="mdp-date">{new Date(row.ngayBatDau + 'T00:00:00').toLocaleDateString('vi-VN')}</td>
                  <td className="mdp-date">{new Date(row.ngayKetThuc + 'T00:00:00').toLocaleDateString('vi-VN')}</td>
                  <td className="mdp-num">{row.soNgayNghi}</td>
                  <td><div style={{ maxWidth: 200, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }} title={row.lyDo}>{row.lyDo}</div></td>
                  <td>
                    <span className={TRANG_THAI_COLOR[row.trangThai] ?? 'mdp-status'}>{row.trangThai}</span>
                    {row.lyDoTuChoi && <div className="mdp-reject-note" title={row.lyDoTuChoi}>↳ {row.lyDoTuChoi}</div>}
                  </td>
                  <td>{row.hoTenNguoiDuyet ?? '—'}</td>
                  <td>
                    {row.trangThai === 'Chờ duyệt' && (
                      <button
                        id={`mdp-btn-delete-${row.id}`}
                        className="mdp-btn mdp-btn--danger mdp-btn--sm"
                        onClick={() => handleDelete(row.id)}
                      >
                        Hủy đơn
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {list.length > PAGE_SIZE && (
            <div className="mdp-pagination">
              <button className="mdp-page-btn" disabled={page === 1} onClick={() => setPage(p => p - 1)}>Trước</button>
              <span className="mdp-page-info">{page}/{totalPages}</span>
              <button className="mdp-page-btn" disabled={page === totalPages} onClick={() => setPage(p => p + 1)}>Sau</button>
            </div>
          )}
        </div>
      )}

      {/* MODAL: Nộp đơn mới */}
      {showModal && (
        <div className="mdp-modal-overlay">
          <div className="mdp-modal">
            <div className="mdp-modal-header">
              <h2>Nộp Đơn Xin Nghỉ</h2>
              <button className="mdp-modal-close" onClick={() => { setShowModal(false); setForm(initialForm); setFormErrors({}); }}>✕</button>
            </div>
            <div className="mdp-modal-body">
              <div className="mdp-form-row">
                <label className="mdp-label">Loại nghỉ <span className="mdp-required">*</span></label>
                <select
                  id="mdp-form-loainghi"
                  className="mdp-select"
                  value={form.loaiNghi}
                  onChange={e => handleFormChange('loaiNghi', e.target.value)}
                >
                  {LOAI_NGHI_OPTIONS.map(opt => (
                    <option key={opt.value} value={opt.value}>{opt.label}</option>
                  ))}
                </select>
              </div>

              <div className="mdp-form-grid">
                <div className="mdp-form-row">
                  <label className="mdp-label">Từ ngày <span className="mdp-required">*</span></label>
                  <input
                    id="mdp-form-ngaybatdau"
                    type="date"
                    className="mdp-input"
                    value={form.ngayBatDau}
                    onChange={e => handleFormChange('ngayBatDau', e.target.value)}
                  />
                  {formErrors.ngayBatDau && <span className="mdp-error-msg">{formErrors.ngayBatDau}</span>}
                </div>
                <div className="mdp-form-row">
                  <label className="mdp-label">Đến ngày <span className="mdp-required">*</span></label>
                  <input
                    id="mdp-form-ngayketthuc"
                    type="date"
                    className="mdp-input"
                    value={form.ngayKetThuc}
                    min={form.ngayBatDau}
                    onChange={e => handleFormChange('ngayKetThuc', e.target.value)}
                  />
                  {formErrors.ngayKetThuc && <span className="mdp-error-msg">{formErrors.ngayKetThuc}</span>}
                </div>
              </div>

              <div className="mdp-form-row">
                <label className="mdp-label">Số ngày nghỉ</label>
                <input
                  id="mdp-form-songay"
                  type="number"
                  className={`mdp-input${formErrors.soNgayNghi ? ' mdp-input--error' : ''}`}
                  min={0.5}
                  step={0.5}
                  value={form.soNgayNghi}
                  onChange={e => setForm(f => ({ ...f, soNgayNghi: +e.target.value }))}
                />
                <small className="mdp-hint">Tự động tính từ khoảng ngày. Có thể điều chỉnh nếu nghỉ nửa ngày.</small>
                {formErrors.soNgayNghi && <span className="mdp-error-msg">{formErrors.soNgayNghi}</span>}
              </div>

              <div className="mdp-form-row">
                <label className="mdp-label">Lý do <span className="mdp-required">*</span></label>
                <textarea
                  id="mdp-form-lydo"
                  className="mdp-textarea"
                  rows={3}
                  placeholder="Nhập lý do xin nghỉ..."
                  value={form.lyDo}
                  onChange={e => setForm(f => ({ ...f, lyDo: e.target.value }))}
                />
                {formErrors.lyDo && <span className="mdp-error-msg">{formErrors.lyDo}</span>}
              </div>

              <div className="mdp-form-row">
                <label className="mdp-label">Tài liệu đính kèm</label>
                <input
                  id="mdp-form-tailieu"
                  type="text"
                  className="mdp-input"
                  placeholder="URL hoặc tên file (tuỳ chọn)"
                  value={form.taiLieuDinhKem}
                  onChange={e => setForm(f => ({ ...f, taiLieuDinhKem: e.target.value }))}
                />
              </div>
            </div>
            <div className="mdp-modal-footer">
              <button className="mdp-btn mdp-btn--outline" onClick={() => { setShowModal(false); setForm(initialForm); setFormErrors({}); }}>
                Hủy
              </button>
              <button id="mdp-btn-submit" className="mdp-btn mdp-btn--primary" onClick={handleSubmit}>
                Nộp đơn
              </button>
            </div>
          </div>
        </div>
      )}

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
    </div>
  );
};
