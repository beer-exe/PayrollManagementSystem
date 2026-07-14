import React, { useState, useEffect, useCallback } from 'react';
import { useChamCong } from '../hooks/useChamCong';
import { chamCongApi } from '../api/chamCongApi';
import { departmentApi } from '../../departments/api/departmentApi';
import type { ChamCongDto, CreateChamCongRequest, UpdateChamCongRequest } from '../types/chamCong.types';
import type { DepartmentDto } from '../../departments/types/department.types';
import { useAuthStore } from '@/store/useAuthStore';
import { ChamCongFormModal } from './ChamCongFormModal';
import { ImportChamCongModal } from './ImportChamCongModal';
import './ChamCongManagement.css';

const LOAI_NGAY_COLOR: Record<string, string> = {
  'Làm đủ ca': 'cc-badge cc-badge--success',
  'Nửa ca': 'cc-badge cc-badge--warning',
  'Đi trễ / Về sớm': 'cc-badge cc-badge--orange',
  'Vắng có phép': 'cc-badge cc-badge--info',
  'Vắng không phép': 'cc-badge cc-badge--danger',
  'Nghỉ lễ': 'cc-badge cc-badge--holiday',
  'Nghỉ cuối tuần': 'cc-badge cc-badge--weekend',
};

const TRANG_THAI_COLOR: Record<string, string> = {
  'Chưa xác nhận': 'cc-status cc-status--pending',
  'Đã xác nhận': 'cc-status cc-status--confirmed',
  'Cần giải trình': 'cc-status cc-status--explain',
};

const now = new Date();

export const ChamCongManagement: React.FC = () => {
  const [thang, setThang] = useState(now.getMonth() + 1);
  const [nam, setNam] = useState(now.getFullYear());
  const [searchCccd, setSearchCccd] = useState('');
  const [activeTab, setActiveTab] = useState<'chi-tiet' | 'tong-hop'>('tong-hop');

  const { user } = useAuthStore();
  const userRole = user?.role || '';

  // Lọc Phòng ban
  const [departments, setDepartments] = useState<DepartmentDto[]>([]);
  const [idPhongBan, setIdPhongBan] = useState('');
  const [pbSearchTerm, setPbSearchTerm] = useState('');
  const [isPbDropdownOpen, setIsPbDropdownOpen] = useState(false);
  const pbDropdownRef = React.useRef<HTMLDivElement>(null);

  useEffect(() => {
    departmentApi.getDepartments()
      .then(res => { if (res.data) setDepartments(res.data); })
      .catch(console.error);

    const handleClickOutside = (e: MouseEvent) => {
      if (pbDropdownRef.current && !pbDropdownRef.current.contains(e.target as Node)) {
        setIsPbDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const filteredDepartments = departments.filter(d => 
    d.tenPb.toLowerCase().includes(pbSearchTerm.toLowerCase())
  );

  const handleSelectDepartment = (d: DepartmentDto | null) => {
    if (d) {
      setIdPhongBan(d.idPb);
      setPbSearchTerm(d.tenPb);
    } else {
      setIdPhongBan('');
      setPbSearchTerm('');
    }
    setIsPbDropdownOpen(false);
  };

  const PAGE_SIZE = 10;
  const [tongHopPage, setTongHopPage] = useState(1);
  const [chiTietPage, setChiTietPage] = useState(1);

  const [showFormModal, setShowFormModal] = useState(false);
  const [showImportModal, setShowImportModal] = useState(false);
  const [editItem, setEditItem] = useState<ChamCongDto | null>(null);

  const [openMenuId, setOpenMenuId] = useState<string | null>(null);
  const [toastMsg, setToastMsg] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  const { list, summary, loading, error, fetchList, fetchSummary, createChamCong, updateChamCong, deleteChamCong } = useChamCong();

  const loadData = useCallback(() => {
    if (activeTab === 'chi-tiet') fetchList(thang, nam, searchCccd || undefined, idPhongBan || undefined);
    else fetchSummary(thang, nam, idPhongBan || undefined);
  }, [activeTab, thang, nam, searchCccd, idPhongBan, fetchList, fetchSummary]);

  useEffect(() => { loadData(); }, [loadData]);

  // Reset pagination when filters change
  useEffect(() => {
    setTongHopPage(1);
    setChiTietPage(1);
  }, [thang, nam, searchCccd, idPhongBan, activeTab]);

  const showToast = (type: 'success' | 'error', text: string) => {
    setToastMsg({ type, text });
    setTimeout(() => setToastMsg(null), 3500);
  };

  const handleCreate = async (data: CreateChamCongRequest) => {
    const err = await createChamCong(data);
    if (err) { showToast('error', err); return false; }
    showToast('success', 'Nhập chấm công thành công!');
    loadData();
    return true;
  };

  const handleUpdate = async (id: string, data: UpdateChamCongRequest) => {
    const err = await updateChamCong(id, data);
    if (err) { showToast('error', err); return false; }
    showToast('success', 'Cập nhật thành công!');
    loadData();
    return true;
  };

  const handleDelete = async (id: string, name: string) => {
    if (!confirm(`Xác nhận xóa bản ghi chấm công của "${name}"?`)) return;
    const err = await deleteChamCong(id);
    if (err) showToast('error', err);
    else { showToast('success', 'Xóa thành công!'); loadData(); }
    setOpenMenuId(null);
  };

  // Tính summary cards từ dữ liệu chi tiết
  const totalPresent = list.filter(r => r.loaiNgayCong === 'Làm đủ ca').length;
  const totalAbsent = list.filter(r => r.loaiNgayCong === 'Vắng không phép').length;
  const totalLate = list.filter(r => r.loaiNgayCong === 'Đi trễ / Về sớm').length;

  // Pagination logic
  const totalTongHopPages = Math.max(1, Math.ceil(summary.length / PAGE_SIZE));
  const currentTongHopList = summary.slice((tongHopPage - 1) * PAGE_SIZE, tongHopPage * PAGE_SIZE);

  const totalChiTietPages = Math.max(1, Math.ceil(list.length / PAGE_SIZE));
  const currentChiTietList = list.slice((chiTietPage - 1) * PAGE_SIZE, chiTietPage * PAGE_SIZE);

  return (
    <div className="cc-page">
      {/* TOAST */}
      {toastMsg && (
        <div className={`cc-toast cc-toast--${toastMsg.type}`}>{toastMsg.text}</div>
      )}

      {/* HEADER */}
      <div className="cc-header">
        <div className="cc-header__left">
          <h1 className="cc-title">Quản lý Chấm Công</h1>
          <p className="cc-subtitle">Theo dõi và quản lý dữ liệu chấm công nhân viên</p>
        </div>
        {userRole !== 'Admin' && (
          <div className="cc-header__actions">
            <button className="cc-btn cc-btn--outline" onClick={() => chamCongApi.downloadTemplate()}>
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" width={16} height={16}>
                <path d="M10.75 2.75a.75.75 0 0 0-1.5 0v8.614L6.295 8.235a.75.75 0 1 0-1.09 1.03l4.25 4.5a.75.75 0 0 0 1.09 0l4.25-4.5a.75.75 0 0 0-1.09-1.03l-2.955 3.129V2.75Z" />
                <path d="M3.5 12.75a.75.75 0 0 0-1.5 0v2.5A2.75 2.75 0 0 0 4.75 18h10.5A2.75 2.75 0 0 0 18 15.25v-2.5a.75.75 0 0 0-1.5 0v2.5c0 .69-.56 1.25-1.25 1.25H4.75c-.69 0-1.25-.56-1.25-1.25v-2.5Z" />
              </svg>
              Tải mẫu CSV
            </button>
            <button className="cc-btn cc-btn--secondary" onClick={() => setShowImportModal(true)}>
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" width={16} height={16}>
                <path d="M9.25 13.25a.75.75 0 0 0 1.5 0V4.636l2.955 3.129a.75.75 0 0 0 1.09-1.03l-4.25-4.5a.75.75 0 0 0-1.09 0l-4.25 4.5a.75.75 0 1 0 1.09 1.03L9.25 4.636v8.614Z" />
                <path d="M3.5 12.75a.75.75 0 0 0-1.5 0v2.5A2.75 2.75 0 0 0 4.75 18h10.5A2.75 2.75 0 0 0 18 15.25v-2.5a.75.75 0 0 0-1.5 0v2.5c0 .69-.56 1.25-1.25 1.25H4.75c-.69 0-1.25-.56-1.25-1.25v-2.5Z" />
              </svg>
              Import CSV
            </button>
            <button className="cc-btn cc-btn--primary" onClick={() => { setEditItem(null); setShowFormModal(true); }}>
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" width={16} height={16}>
                <path d="M10.75 4.75a.75.75 0 0 0-1.5 0v4.5h-4.5a.75.75 0 0 0 0 1.5h4.5v4.5a.75.75 0 0 0 1.5 0v-4.5h4.5a.75.75 0 0 0 0-1.5h-4.5v-4.5Z" />
              </svg>
              Nhập thủ công
            </button>
          </div>
        )}
      </div>

      {/* FILTER BAR */}
      <div className="cc-filter-bar">
        <div className="cc-filter-group">
          <label className="cc-filter-label">Tháng</label>
          <select className="cc-select" value={thang} onChange={e => setThang(+e.target.value)}>
            {Array.from({ length: 12 }, (_, i) => i + 1).map(m => (
              <option key={m} value={m}>Tháng {m}</option>
            ))}
          </select>
        </div>
        <div className="cc-filter-group">
          <label className="cc-filter-label">Năm</label>
          <select className="cc-select" value={nam} onChange={e => setNam(+e.target.value)}>
            {[2024, 2025, 2026, 2027].map(y => (
              <option key={y} value={y}>{y}</option>
            ))}
          </select>
        </div>
        <div className="cc-filter-group" style={{ flex: 1, minWidth: 200 }}>
          <label className="cc-filter-label">Phòng ban</label>
          <div className="cc-dropdown-select-wrap" ref={pbDropdownRef}>
            <input
              className="cc-input"
              style={{ width: '100%' }}
              placeholder="-- Tất cả --"
              value={pbSearchTerm}
              onChange={e => {
                setPbSearchTerm(e.target.value);
                setIdPhongBan(''); // Xóa selection cũ khi user gõ search mới
                setIsPbDropdownOpen(true);
              }}
              onFocus={() => {
                setPbSearchTerm('');
                setIdPhongBan('');
                setIsPbDropdownOpen(true);
              }}
              autoComplete="off"
            />
            {isPbDropdownOpen && (
              <ul className="cc-dropdown-select-list">
                <li 
                  className={!idPhongBan ? 'selected' : ''}
                  onClick={() => handleSelectDepartment(null)}
                >
                  -- Tất cả --
                </li>
                {filteredDepartments.length > 0 ? (
                  filteredDepartments.map(d => (
                    <li 
                      key={d.idPb} 
                      className={idPhongBan === d.idPb ? 'selected' : ''}
                      onClick={() => handleSelectDepartment(d)}
                    >
                      {d.tenPb}
                    </li>
                  ))
                ) : (
                  <li className="cc-empty-option">Không tìm thấy phòng ban</li>
                )}
              </ul>
            )}
          </div>
        </div>
        {activeTab === 'chi-tiet' && (
          <div className="cc-filter-group">
            <label className="cc-filter-label">Tìm kiếm</label>
            <input
              className="cc-input"
              placeholder="Lọc theo Tên, CCCD..."
              value={searchCccd}
              onChange={e => setSearchCccd(e.target.value)}
            />
          </div>
        )}
      </div>

      {/* SUMMARY CARDS (chỉ hiện ở tab chi tiết) */}
      {activeTab === 'chi-tiet' && list.length > 0 && (
        <div className="cc-cards">
          <div className="cc-card cc-card--green">
            <span className="cc-card__num">{totalPresent}</span>
            <span className="cc-card__label">Làm đủ ca</span>
          </div>
          <div className="cc-card cc-card--orange">
            <span className="cc-card__num">{totalLate}</span>
            <span className="cc-card__label">Đi trễ / Về sớm</span>
          </div>
          <div className="cc-card cc-card--red">
            <span className="cc-card__num">{totalAbsent}</span>
            <span className="cc-card__label">Vắng không phép</span>
          </div>
          <div className="cc-card cc-card--purple">
            <span className="cc-card__num">{list.length}</span>
            <span className="cc-card__label">Tổng bản ghi</span>
          </div>
        </div>
      )}

      {/* TABS */}
      <div className="cc-tabs">
        <button className={`cc-tab ${activeTab === 'tong-hop' ? 'active' : ''}`} onClick={() => setActiveTab('tong-hop')}>
          Tổng hợp ngày công
        </button>
        <button className={`cc-tab ${activeTab === 'chi-tiet' ? 'active' : ''}`} onClick={() => setActiveTab('chi-tiet')}>
          Chi tiết chấm công
        </button>
      </div>

      {/* CONTENT */}
      {loading ? (
        <div className="cc-loading">
          <div className="cc-spinner" />
          <span>Đang tải dữ liệu...</span>
        </div>
      ) : error ? (
        <div className="cc-error">{error}</div>
      ) : activeTab === 'tong-hop' ? (
        /* BẢNG TỔNG HỢP */
        <div className="cc-table-wrap">
          <table className="cc-table">
            <thead>
              <tr>
                <th>Nhân viên</th>
                <th>Phòng ban</th>
                <th>Ngày chuẩn</th>
                <th>Ngày thực tế</th>
                <th>Nghỉ lễ</th>
                <th>Vắng không phép</th>
                <th>Cần giải trình</th>
              </tr>
            </thead>
            <tbody>
              {summary.length === 0 ? (
                <tr><td colSpan={7} className="cc-empty">Không có dữ liệu tổng hợp cho tháng {thang}/{nam}</td></tr>
              ) : currentTongHopList.map(row => (
                <tr key={row.cccdNhanVien}>
                  <td>
                    <div className="cc-nv-name">{row.hoTenNhanVien}</div>
                    <div className="cc-nv-cccd">{row.cccdNhanVien}</div>
                  </td>
                  <td>{row.tenPhongBan ?? '—'}</td>
                  <td className="cc-num">{row.ngayCongChuan}</td>
                  <td className="cc-num">
                    <span className={row.tongNgayCongThucTe >= row.ngayCongChuan ? 'cc-num--ok' : 'cc-num--warn'}>
                      {row.tongNgayCongThucTe.toFixed(2)}
                    </span>
                  </td>
                  <td className="cc-num">{row.ngayNghiLe}</td>
                  <td className="cc-num">
                    {row.ngayVangKhongPhep > 0
                      ? <span className="cc-num--danger">{row.ngayVangKhongPhep}</span>
                      : <span>0</span>}
                  </td>
                  <td className="cc-num">
                    {row.ngayCanGiaiTrinh > 0
                      ? <span className="cc-num--warn">{row.ngayCanGiaiTrinh}</span>
                      : <span>0</span>}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {summary.length > 0 && (
            <div className="cc-pagination">
              <button 
                className="cc-page-btn" 
                disabled={tongHopPage === 1} 
                onClick={() => setTongHopPage(p => p - 1)}
              >
                Trước
              </button>
              <span className="cc-page-info">{tongHopPage}/{totalTongHopPages}</span>
              <button 
                className="cc-page-btn" 
                disabled={tongHopPage === totalTongHopPages} 
                onClick={() => setTongHopPage(p => p + 1)}
              >
                Sau
              </button>
            </div>
          )}
        </div>
      ) : (
        /* BẢNG CHI TIẾT */
        <div className="cc-table-wrap">
          <table className="cc-table">
            <thead>
              <tr>
                <th>Ngày</th>
                <th>Nhân viên</th>
                <th>Giờ vào</th>
                <th>Giờ ra</th>
                <th>Số giờ</th>
                <th>Ngày công</th>
                <th>Loại ngày</th>
                <th>Trạng thái</th>
                <th>Ghi chú</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {list.length === 0 ? (
                <tr><td colSpan={10} className="cc-empty">Không có dữ liệu chấm công cho kỳ này</td></tr>
              ) : currentChiTietList.map(row => (
                <tr key={row.id}>
                  <td className="cc-date">
                    {new Date(row.ngayChamCong + 'T00:00:00').toLocaleDateString('vi-VN')}
                  </td>
                  <td>
                    <div className="cc-nv-name">{row.hoTenNhanVien}</div>
                    <div className="cc-nv-cccd">{row.cccdNhanVien}</div>
                  </td>
                  <td>{row.gioVao ?? '—'}</td>
                  <td>{row.gioRa ?? '—'}</td>
                  <td className="cc-num">{row.soGioLamThucTe.toFixed(1)}h</td>
                  <td className="cc-num">{row.soNgayCong.toFixed(2)}</td>
                  <td><span className={LOAI_NGAY_COLOR[row.loaiNgayCong] ?? 'cc-badge'}>{row.loaiNgayCong}</span></td>
                  <td><span className={TRANG_THAI_COLOR[row.trangThai] ?? 'cc-status'}>{row.trangThai}</span></td>
                  <td className="cc-note">{row.ghiChu ?? '—'}</td>
                  <td className="cc-actions-cell">
                    {userRole !== 'Admin' && (
                      <div className="cc-dropdown-wrap">
                        <button
                          className="cc-actions-btn"
                          onClick={() => setOpenMenuId(openMenuId === row.id ? null : row.id)}
                        >•••</button>
                        {openMenuId === row.id && (
                          <div className="cc-dropdown">
                            <button onClick={() => { setEditItem(row); setShowFormModal(true); setOpenMenuId(null); }}>
                              ✏️ Chỉnh sửa
                            </button>
                            <button className="cc-dropdown__danger" onClick={() => handleDelete(row.id, row.hoTenNhanVien)}>
                              🗑️ Xóa
                            </button>
                          </div>
                        )}
                      </div>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {list.length > 0 && (
            <div className="cc-pagination">
              <button 
                className="cc-page-btn" 
                disabled={chiTietPage === 1} 
                onClick={() => setChiTietPage(p => p - 1)}
              >
                Trước
              </button>
              <span className="cc-page-info">{chiTietPage}/{totalChiTietPages}</span>
              <button 
                className="cc-page-btn" 
                disabled={chiTietPage === totalChiTietPages} 
                onClick={() => setChiTietPage(p => p + 1)}
              >
                Sau
              </button>
            </div>
          )}
        </div>
      )}

      {/* MODALS */}
      {showFormModal && (
        <ChamCongFormModal
          editItem={editItem}
          onClose={() => { setShowFormModal(false); setEditItem(null); }}
          onCreate={handleCreate}
          onUpdate={handleUpdate}
        />
      )}
      {showImportModal && (
        <ImportChamCongModal
          onClose={() => setShowImportModal(false)}
          onSuccess={loadData}
        />
      )}
    </div>
  );
};
