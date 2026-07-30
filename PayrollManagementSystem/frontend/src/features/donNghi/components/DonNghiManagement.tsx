import React, { useState, useEffect, useCallback, useRef } from 'react';
import { useDonNghi } from '../hooks/useDonNghi';
import { departmentApi } from '../../departments/api/departmentApi';
import { useAuthStore } from '@/store/useAuthStore';
import type { DonNghiDto, NgayPhepDto, UpdateNgayPhepRequest } from '../types/donNghi.types';
import type { DepartmentDto } from '../../departments/types/department.types';
import { DonNghiFormModal } from './DonNghiFormModal';
import { employeeApi } from '../../employees/api/employeeApi';
import { workScheduleApi } from '../../workSchedule/api/workScheduleApi';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { Toast } from '../../../components/Toast/Toast';
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


export const DonNghiManagement: React.FC = () => {
  const { user } = useAuthStore();
  const userRole = user?.role || '';
  const isHR = userRole === 'HR';

  const [activeTab, setActiveTab] = useState<'don-nghi' | 'ngay-phep'>('don-nghi');
  const [thang, setThang] = useState(now.getMonth() + 1);
  const [nam, setNam] = useState(now.getFullYear());
  const [filterTrangThai, setFilterTrangThai] = useState('');


  // Phòng ban filter
  const [departments, setDepartments] = useState<DepartmentDto[]>([]);
  const [idPhongBan, setIdPhongBan] = useState('');
  const [pbSearchTerm, setPbSearchTerm] = useState('');
  const [isPbOpen, setIsPbOpen] = useState(false);
  const pbRef = useRef<HTMLDivElement>(null);

  // Navigation between tabs handled outside of pagination
  // Pagination handled by useDataTable

  // Modals
  const [showFormModal, setShowFormModal] = useState(false);
  const [openMenuId, setOpenMenuId] = useState<string | null>(null);
  const [tuChoiId, setTuChoiId] = useState<string | null>(null);
  const [tuChoiLyDo, setTuChoiLyDo] = useState('');
  const [ngayPhepEdit, setNgayPhepEdit] = useState<NgayPhepDto | null>(null);
  const [ngayPhepForm, setNgayPhepForm] = useState({ cccd: '', nam: now.getFullYear(), tong: 12 });
  const [showNgayPhepModal, setShowNgayPhepModal] = useState(false);

  // --- Employee search for Modal ---
  const [empList, setEmpList] = useState<{ cccd: string; hoTen: string; tenPhongBan?: string | null }[]>([]);
  const [cccdSearchTerm, setCccdSearchTerm] = useState('');
  const [cccdDropdownOpen, setCccdDropdownOpen] = useState(false);
  const cccdDropdownRef = useRef<HTMLDivElement>(null);

  const [validYears, setValidYears] = useState<number[]>([]);

  const { list, ngayPhepList, loading, fetchList, fetchNgayPhep, createDonNghi, duyetDonNghi, tuChoiDonNghi, deleteDonNghi, updateNgayPhep, huyDonNghiDaDuyet, toast, setToast, showToast } = useDonNghi();

  const {
    currentData: currentDonNghiList,
    allFilteredAndSortedData: allDonNghi,
    currentPage: donNghiPage,
    totalPages: totalDonNghiPages,
    setCurrentPage: setDonNghiPage,
    sortKey: donNghiSortKey,
    sortDirection: donNghiSortDirection,
    handleSort: handleDonNghiSort,
    searchTerm: donNghiSearchTerm,
    setSearchTerm: setDonNghiSearchTerm
  } = useDataTable<DonNghiDto>({
    data: list,
    initialPageSize: 10,
    searchableFields: ['hoTenNhanVien', 'cccdNhanVien']
  });

  const {
    currentData: currentNgayPhepList,
    allFilteredAndSortedData: allNgayPhep,
    currentPage: ngayPhepPage,
    totalPages: totalNgayPhepPages,
    setCurrentPage: setNgayPhepPage,
    sortKey: ngayPhepSortKey,
    sortDirection: ngayPhepSortDirection,
    handleSort: handleNgayPhepSort,
    searchTerm: ngayPhepSearchTerm,
    setSearchTerm: setNgayPhepSearchTerm
  } = useDataTable<NgayPhepDto>({
    data: ngayPhepList,
    initialPageSize: 10,
    searchableFields: ['hoTenNhanVien', 'cccdNhanVien']
  });

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

  useEffect(() => {
    if (showNgayPhepModal && empList.length === 0) {
      employeeApi.getEmployees({ PageNumber: 1, PageSize: 1000 })
        .then(res => setEmpList(res.data || []))
        .catch(console.error);
    }
  }, [showNgayPhepModal, empList.length]);

  useEffect(() => {
    workScheduleApi.getAll()
      .then(res => {
        if (res.data) setValidYears(Array.from(new Set(res.data.map(w => w.nam))).sort((a, b) => b - a));
      })
      .catch(console.error);
  }, []);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (cccdDropdownRef.current && !cccdDropdownRef.current.contains(e.target as Node)) {
        setCccdDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const filteredEmpList = empList.filter(e =>
    !cccdSearchTerm ||
    e.hoTen?.toLowerCase().includes(cccdSearchTerm.toLowerCase()) ||
    e.cccd?.includes(cccdSearchTerm)
  );



  // Removed custom filtering and pagination array slicing since useDataTable handles it

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

  const handleHuyDaDuyet = async (id: string, name: string) => {
    if (!confirm(`Xác nhận hủy đơn nghỉ ĐÃ DUYỆT của "${name}"?\n(Thao tác này sẽ hoàn trả lại ngày phép và cập nhật lại lịch chấm công)`)) return;
    const err = await huyDonNghiDaDuyet(id);
    if (err) showToast('error', err);
    else { showToast('success', 'Hủy đơn thành công!'); loadData(); }
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
    else { showToast('success', 'Cập nhật ngày phép thành công!'); setShowNgayPhepModal(false); loadData(); }
  };

  const openNgayPhepModal = async (row?: NgayPhepDto) => {
    try {
      let years = validYears;
      if (years.length === 0) {
        const res = await workScheduleApi.getAll();
        years = Array.from(new Set((res.data || []).map(w => w.nam))).sort((a, b) => b - a);
        setValidYears(years);
      }
      
      if (years.length === 0) {
        showToast('error', 'Chưa có lịch làm việc nào được tạo. Vui lòng tạo lịch làm việc trước khi cấu hình ngày phép!');
        return;
      }

      if (row) {
        setNgayPhepForm({ cccd: row.cccdNhanVien, nam: row.nam, tong: row.tongNgayPhep });
        setCccdSearchTerm(`${row.hoTenNhanVien} - ${row.cccdNhanVien}`);
        setNgayPhepEdit(row);
      } else {
        setNgayPhepForm({ cccd: '', nam: years.includes(now.getFullYear()) ? now.getFullYear() : years[0], tong: 12 });
        setCccdSearchTerm('');
        setNgayPhepEdit(null);
      }
      setShowNgayPhepModal(true);
    } catch (_: unknown) {
      showToast('error', 'Lỗi khi kiểm tra dữ liệu năm!');
    }
  };

  const handleExportExcel = () => {
    if (activeTab === 'don-nghi') {
      const columns: ExportColumn<DonNghiDto>[] = [
        { header: 'Mã NV', key: 'cccdNhanVien' },
        { header: 'Họ Tên', key: 'hoTenNhanVien' },
        { header: 'Phòng ban', key: 'tenPhongBan' },
        { header: 'Loại nghỉ', key: 'loaiNghi' },
        { header: 'Số ngày', key: 'soNgayNghi' },
        { header: 'Trạng thái', key: 'trangThai' },
      ];
      exportToExcel(allDonNghi, columns, 'DanhSachDonNghi');
    } else {
      const columns: ExportColumn<NgayPhepDto>[] = [
        { header: 'Mã NV', key: 'cccdNhanVien' },
        { header: 'Họ Tên', key: 'hoTenNhanVien' },
        { header: 'Phòng ban', key: 'tenPhongBan' },
        { header: 'Năm', key: 'nam' },
        { header: 'Tổng phép', key: 'tongNgayPhep' },
        { header: 'Đã dùng', key: 'daSuDung' },
        { header: 'Còn lại', key: 'conLai' },
      ];
      exportToExcel(allNgayPhep, columns, 'DanhSachNgayPhep');
    }
  };

  const handleExportPdf = () => {
    if (activeTab === 'don-nghi') {
      const columns: ExportColumn<DonNghiDto>[] = [
        { header: 'Mã NV', key: 'cccdNhanVien' },
        { header: 'Họ Tên', key: 'hoTenNhanVien' },
        { header: 'Phòng ban', key: 'tenPhongBan' },
        { header: 'Loại nghỉ', key: 'loaiNghi' },
        { header: 'Số ngày', key: 'soNgayNghi' },
        { header: 'Trạng thái', key: 'trangThai' },
      ];
      exportToPdf(allDonNghi, columns, 'DanhSachDonNghi', 'Danh sách đơn nghỉ');
    } else {
      const columns: ExportColumn<NgayPhepDto>[] = [
        { header: 'Mã NV', key: 'cccdNhanVien' },
        { header: 'Họ Tên', key: 'hoTenNhanVien' },
        { header: 'Phòng ban', key: 'tenPhongBan' },
        { header: 'Năm', key: 'nam' },
        { header: 'Tổng phép', key: 'tongNgayPhep' },
        { header: 'Đã dùng', key: 'daSuDung' },
        { header: 'Còn lại', key: 'conLai' },
      ];
      exportToPdf(allNgayPhep, columns, 'DanhSachNgayPhep', 'Danh sách ngày phép');
    }
  };

  return (
    <div className="dn-page">
      {/* TOAST */}


      {/* HEADER */}
      <div className="dn-header">
        <div className="dn-header__left">
          <h1 className="dn-title">🏖️ Quản lý Đơn Xin Nghỉ</h1>
          <p className="dn-subtitle">Quản lý đơn nghỉ và ngày phép năm của nhân viên</p>
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
          <select className="dn-select" value={nam} onChange={e => setNam(+e.target.value)} disabled={validYears.length === 0}>
            {validYears.length > 0 
              ? validYears.map(y => <option key={y} value={y}>{y}</option>) 
              : <option value={now.getFullYear()}>Chưa có lịch làm việc nào được tạo.</option>}
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
          <div className="dn-filter-group">
            <label className="dn-filter-label">Trạng thái</label>
            <select className="dn-select" value={filterTrangThai} onChange={e => setFilterTrangThai(e.target.value)}>
              <option value="">-- Tất cả --</option>
              <option value="CHO_DUYET">Chờ duyệt</option>
              <option value="DA_DUYET">Đã duyệt</option>
              <option value="TU_CHOI">Từ chối</option>
            </select>
          </div>
        )}
        <div className="dn-filter-group">
          <label className="dn-filter-label">Tìm kiếm</label>
          <input className="dn-input" placeholder="Tên, CCCD..." value={activeTab === 'don-nghi' ? donNghiSearchTerm : ngayPhepSearchTerm} onChange={e => activeTab === 'don-nghi' ? setDonNghiSearchTerm(e.target.value) : setNgayPhepSearchTerm(e.target.value)} />
        </div>
        <div className="dn-filter-group" style={{ display: 'flex', alignItems: 'flex-end' }}>
          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
        </div>
      </div>

      {/* TABS */}
      <div className="dn-tabs">
        <button className={`dn-tab ${activeTab === 'don-nghi' ? 'active' : ''}`} onClick={() => setActiveTab('don-nghi')}>
          Danh sách đơn nghỉ
        </button>
        <button className={`dn-tab ${activeTab === 'ngay-phep' ? 'active' : ''}`} onClick={() => setActiveTab('ngay-phep')}>
          Ngày phép năm
        </button>
      </div>

      {/* CONTENT */}
      {loading ? (
        <div className="dn-loading"><div className="dn-spinner" /><span>Đang tải...</span></div>
      ) : activeTab === 'don-nghi' ? (
        /* BẢNG ĐƠN NGHỈ */
        <div className="dn-table-wrap">
          <table className="dn-table">
            <thead>
              <tr>
                <SortableHeader label="Nhân viên" sortKey="hoTenNhanVien" currentSortKey={donNghiSortKey} currentSortDirection={donNghiSortDirection} onSort={handleDonNghiSort} />
                <SortableHeader label="Phòng ban" sortKey="tenPhongBan" currentSortKey={donNghiSortKey} currentSortDirection={donNghiSortDirection} onSort={handleDonNghiSort} />
                <SortableHeader label="Loại nghỉ" sortKey="loaiNghi" currentSortKey={donNghiSortKey} currentSortDirection={donNghiSortDirection} onSort={handleDonNghiSort} />
                <SortableHeader label="Từ ngày" sortKey="ngayBatDau" currentSortKey={donNghiSortKey} currentSortDirection={donNghiSortDirection} onSort={handleDonNghiSort} />
                <SortableHeader label="Đến ngày" sortKey="ngayKetThuc" currentSortKey={donNghiSortKey} currentSortDirection={donNghiSortDirection} onSort={handleDonNghiSort} />
                <SortableHeader label="Số ngày" sortKey="soNgayNghi" currentSortKey={donNghiSortKey} currentSortDirection={donNghiSortDirection} onSort={handleDonNghiSort} />
                <th>Lý do</th>
                <SortableHeader label="Trạng thái" sortKey="trangThai" currentSortKey={donNghiSortKey} currentSortDirection={donNghiSortDirection} onSort={handleDonNghiSort} />
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
                  <td className="dn-date">{new Date(row.ngayBatDau + 'T00:00:00').toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })}</td>
                  <td className="dn-date">{new Date(row.ngayKetThuc + 'T00:00:00').toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })}</td>
                  <td className="dn-num">{row.soNgayNghi}</td>
                  <td><div style={{ maxWidth: 200, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }} title={row.lyDo}>{row.lyDo}</div></td>
                  <td>
                    <span className={TRANG_THAI_COLOR[row.trangThai] ?? 'dn-status'}>{row.trangThai}</span>
                    {row.lyDoTuChoi && <div style={{ fontSize: 11, color: 'var(--danger-text)', marginTop: 3, maxWidth: 160, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }} title={row.lyDoTuChoi}>↳ {row.lyDoTuChoi}</div>}
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
                            {row.trangThai === 'Đã duyệt' && (
                              <button className="dn-dropdown__danger" onClick={() => handleHuyDaDuyet(row.id, row.hoTenNhanVien)}>🔙 Hủy đơn</button>
                            )}
                            {row.trangThai !== 'Chờ duyệt' && row.trangThai !== 'Đã duyệt' && (
                              <button disabled className="dn-dropdown__disabled">Không có thao tác</button>
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
          {totalDonNghiPages > 0 && (
            <div className="dn-pagination">
              <button className="dn-page-btn" disabled={donNghiPage === 1} onClick={() => setDonNghiPage(p => p - 1)}>Trước</button>
              <span className="dn-page-info">{donNghiPage}/{totalDonNghiPages}</span>
              <button className="dn-page-btn" disabled={donNghiPage === totalDonNghiPages} onClick={() => setDonNghiPage(p => p + 1)}>Sau</button>
            </div>
          )}
        </div>
      ) : (
        /* BẢNG NGÀY PHÉP */
        <div className="dn-table-wrap">
          <table className="dn-table">
            <thead>
              <tr>
                <SortableHeader label="Nhân viên" sortKey="hoTenNhanVien" currentSortKey={ngayPhepSortKey} currentSortDirection={ngayPhepSortDirection} onSort={handleNgayPhepSort} />
                <SortableHeader label="Phòng ban" sortKey="tenPhongBan" currentSortKey={ngayPhepSortKey} currentSortDirection={ngayPhepSortDirection} onSort={handleNgayPhepSort} />
                <SortableHeader label="Năm" sortKey="nam" currentSortKey={ngayPhepSortKey} currentSortDirection={ngayPhepSortDirection} onSort={handleNgayPhepSort} />
                <SortableHeader label="Tổng phép" sortKey="tongNgayPhep" currentSortKey={ngayPhepSortKey} currentSortDirection={ngayPhepSortDirection} onSort={handleNgayPhepSort} />
                <SortableHeader label="Đã dùng" sortKey="daSuDung" currentSortKey={ngayPhepSortKey} currentSortDirection={ngayPhepSortDirection} onSort={handleNgayPhepSort} />
                <SortableHeader label="Còn lại" sortKey="conLai" currentSortKey={ngayPhepSortKey} currentSortDirection={ngayPhepSortDirection} onSort={handleNgayPhepSort} />
                {isHR && <th></th>}
              </tr>
            </thead>
            <tbody>
              {currentNgayPhepList.length === 0 ? (
                <tr><td colSpan={isHR ? 7 : 6} className="dn-empty">Chưa có cấu hình ngày phép cho năm {nam}</td></tr>
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

      {/* MODAL: Cấu hình ngày phép */}
      {showNgayPhepModal && (
        <div className="dn-modal-overlay">
          <div className="dn-modal dn-modal--sm">
            <div className="dn-modal-header">
              <h2>{ngayPhepEdit ? 'Cập nhật ngày phép' : 'Tạo ngày phép'}</h2>
              <button className="dn-modal-close" onClick={() => setShowNgayPhepModal(false)}>✕</button>
            </div>
            <div className="dn-modal-body">
              <div className="dn-form-row">
                <label className="dn-label">Nhân viên<span className="dn-required">*</span></label>
                <div className="dn-dropdown-select-wrap" ref={cccdDropdownRef}>
                  <input
                    className="dn-input"
                    style={{ width: '100%' }}
                    placeholder="Tìm theo tên hoặc CCCD..."
                    value={cccdSearchTerm}
                    onChange={e => { setCccdSearchTerm(e.target.value); setNgayPhepForm(f => ({ ...f, cccd: '' })); setCccdDropdownOpen(true); }}
                    onFocus={() => { if (!ngayPhepEdit) { setCccdSearchTerm(''); setNgayPhepForm(f => ({ ...f, cccd: '' })); setCccdDropdownOpen(true); } }}
                    disabled={!!ngayPhepEdit}
                    autoComplete="off"
                  />
                  {cccdDropdownOpen && !ngayPhepEdit && (
                    <ul className="dn-dropdown-select-list">
                      {filteredEmpList.length > 0
                        ? filteredEmpList.map(e => (
                          <li key={e.cccd} className={ngayPhepForm.cccd === e.cccd ? 'selected' : ''}
                            onClick={() => { setNgayPhepForm(f => ({ ...f, cccd: e.cccd })); setCccdSearchTerm(`${e.hoTen} - ${e.cccd}`); setCccdDropdownOpen(false); }}>
                            {e.hoTen} - {e.cccd}
                          </li>
                        ))
                        : <li className="dn-empty-option">Không tìm thấy nhân viên</li>}
                    </ul>
                  )}
                </div>
              </div>
              <div className="dn-form-row">
                <label className="dn-label">Năm</label>
                <select className="dn-select" value={ngayPhepForm.nam} onChange={e => setNgayPhepForm(f => ({ ...f, nam: +e.target.value }))} disabled={!!ngayPhepEdit}>
                  {validYears.length > 0 ? validYears.map(y => <option key={y} value={y}>{y}</option>) : <option value={now.getFullYear()}>{now.getFullYear()}</option>}
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
