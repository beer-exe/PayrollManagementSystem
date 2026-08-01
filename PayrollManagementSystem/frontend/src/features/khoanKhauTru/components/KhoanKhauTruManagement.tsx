import React, { useState, useEffect, useRef, useCallback } from 'react';
import './KhoanKhauTruManagement.css';
import { khoanKhauTruApi } from '../api/khoanKhauTruApi';
import {
  KhoanKhauTruDto,
  CreateKhoanKhauTruRequest,
  UpdateKhoanKhauTruRequest,
  LoaiCongThuc,
} from '../types';
import { useDataTable } from '../../../hooks/useDataTable';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';

const LOAI_CONG_THUC_OPTIONS: { value: LoaiCongThuc; label: string }[] = [
  { value: 'TY_LE_PHAN_TRAM', label: 'Tỷ lệ phần trăm' },
  { value: 'SO_TIEN_CO_DINH', label: 'Số tiền cố định' },
];

const formatCurrency = (val: number) =>
  new Intl.NumberFormat('vi-VN').format(val);

interface FormState {
  tenKhoanKhauTru: string;
  loaiCongThuc: LoaiCongThuc;
  giaTri: string;
  ghiChu: string;
  isActive: boolean;
}

const defaultForm: FormState = {
  tenKhoanKhauTru: '',
  loaiCongThuc: 'TY_LE_PHAN_TRAM',
  giaTri: '',
  ghiChu: '',
  isActive: true,
};

export const KhoanKhauTruManagement: React.FC = () => {
  const [items, setItems] = useState<KhoanKhauTruDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Modal
  const [modalOpen, setModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<KhoanKhauTruDto | null>(null);
  const [form, setForm] = useState<FormState>(defaultForm);
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);

  // Action dropdown
  const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Data Table Hook
  const {
    currentData: paginatedItems,
    allFilteredAndSortedData,
    currentPage,
    totalPages,
    setCurrentPage,
    sortKey,
    sortDirection,
    handleSort,
    searchTerm,
    setSearchTerm
  } = useDataTable<KhoanKhauTruDto>({
    data: items,
    initialPageSize: 10,
    searchableFields: ['tenKhoanKhauTru', 'ghiChu']
  });

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const res = await khoanKhauTruApi.getList();
      setItems(res.data || []);
    } catch (err: any) {
      setError(err?.response?.data?.Message || 'Không thể tải danh sách khoản khấu trừ.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  useEffect(() => {
    const handleOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setOpenDropdownId(null);
      }
    };
    document.addEventListener('mousedown', handleOutside);
    return () => document.removeEventListener('mousedown', handleOutside);
  }, []);

  const openCreateModal = () => {
    setEditingItem(null);
    setForm(defaultForm);
    setFormError('');
    setModalOpen(true);
  };

  const openEditModal = (item: KhoanKhauTruDto) => {
    setEditingItem(item);
    const loai = LOAI_CONG_THUC_OPTIONS.find(o => o.label === item.loaiCongThuc);
    setForm({
      tenKhoanKhauTru: item.tenKhoanKhauTru,
      loaiCongThuc: loai?.value ?? 'TY_LE_PHAN_TRAM',
      giaTri: String(item.giaTri),
      ghiChu: item.ghiChu ?? '',
      isActive: item.isActive,
    });
    setFormError('');
    setModalOpen(true);
    setOpenDropdownId(null);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingItem(null);
    setForm(defaultForm);
    setFormError('');
  };

  const validateForm = (): boolean => {
    if (!form.tenKhoanKhauTru.trim()) {
      setFormError('Tên khoản khấu trừ không được để trống.');
      return false;
    }
    const val = parseFloat(form.giaTri);
    if (isNaN(val) || val <= 0) {
      setFormError('Giá trị phải là số lớn hơn 0.');
      return false;
    }
    return true;
  };

  const handleSave = async () => {
    setFormError('');
    if (!validateForm()) return;

    setSaving(true);
    try {
      const payload: CreateKhoanKhauTruRequest | UpdateKhoanKhauTruRequest = {
        tenKhoanKhauTru: form.tenKhoanKhauTru.trim(),
        loaiCongThuc: form.loaiCongThuc,
        giaTri: parseFloat(form.giaTri),
        ghiChu: form.ghiChu.trim() || undefined,
        isActive: form.isActive,
      };

      if (editingItem) {
        await khoanKhauTruApi.update(editingItem.idKhoanKhauTru, payload as UpdateKhoanKhauTruRequest);
      } else {
        await khoanKhauTruApi.create(payload as CreateKhoanKhauTruRequest);
      }

      closeModal();
      fetchData();
    } catch (err: any) {
      setFormError(err?.response?.data?.Message || 'Đã xảy ra lỗi, vui lòng thử lại.');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (item: KhoanKhauTruDto) => {
    if (!window.confirm(`Xóa khoản khấu trừ "${item.tenKhoanKhauTru}"?\nHành động này có thể hoàn tác bằng cách liên hệ quản trị viên.`)) return;
    setOpenDropdownId(null);
    try {
      await khoanKhauTruApi.delete(item.idKhoanKhauTru);
      fetchData();
    } catch (err: any) {
      alert(err?.response?.data?.Message || 'Xóa thất bại.');
    }
  };

  const handleExportExcel = () => {
    const columns: ExportColumn<KhoanKhauTruDto>[] = [
      { header: 'Tên khoản khấu trừ', key: 'tenKhoanKhauTru' },
      { header: 'Loại công thức', key: 'loaiCongThuc' },
      { header: 'Giá trị', key: 'giaTri' },
      { header: 'Ghi chú', key: 'ghiChu' },
      { header: 'Trạng thái', key: 'isActive', formatter: (val) => val ? 'Đang kích hoạt' : 'Tạm dừng' },
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'Danh_Sach_Khoan_Khau_Tru.xlsx');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<KhoanKhauTruDto>[] = [
      { header: 'Tên khoản khấu trừ', key: 'tenKhoanKhauTru' },
      { header: 'Loại công thức', key: 'loaiCongThuc' },
      { header: 'Giá trị', key: 'giaTri' },
      { header: 'Ghi chú', key: 'ghiChu' },
      { header: 'Trạng thái', key: 'isActive', formatter: (val) => val ? 'Đang kích hoạt' : 'Tạm dừng' },
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'Danh_Sach_Khoan_Khau_Tru.pdf', 'DANH SÁCH KHOẢN KHẤU TRỪ');
  };

  return (
    <div className="kkt-container">
      {/* Header */}
      <div className="kkt-header">
        <div className="kkt-header-title">
          <h2>✂️ Cấu hình Khoản Khấu Trừ</h2>
          <p>Quản lý các khoản khấu trừ áp dụng trong tính lương</p>
        </div>
        <button className="kkt-btn kkt-btn-primary" onClick={openCreateModal}>
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 18, height: 18 }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
          </svg>
          Thêm khoản
        </button>
      </div>

      {/* Error Banner */}
      {error && (
        <div style={{ color: 'var(--danger)', marginBottom: '1rem', padding: '0.5rem 1rem', background: '#fee2e2', borderRadius: '8px' }}>
          {error}
        </div>
      )}

      {/* Controls */}
      <div className="kkt-controls-wrapper">
        <div className="kkt-filters">
          <div className="kkt-input-wrapper">
            <svg xmlns="http://www.w3.org/2000/svg" className="kkt-input-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" width={16} height={16}>
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            <input 
              type="text" 
              className="kkt-input" 
              placeholder="Tìm kiếm khoản khấu trừ..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
        </div>

        <div className="kkt-table-container custom-scrollbar">
          <table className="kkt-table">
            <thead>
              <tr>
                <SortableHeader label="Tên khoản khấu trừ" sortKey="tenKhoanKhauTru" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Cách tính" sortKey="loaiCongThuc" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Giá trị" sortKey="giaTri" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Ghi chú" sortKey="ghiChu" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Trạng thái" sortKey="isActive" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                <th style={{ textAlign: 'right', width: 90 }}>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={6}>
                    <div className="kkt-loader">
                      <div className="kkt-spinner" />
                    </div>
                  </td>
                </tr>
              ) : paginatedItems.length === 0 ? (
                <tr>
                  <td colSpan={6}>
                    <div className="kkt-empty">
                      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1} stroke="currentColor" style={{ width: 48, height: 48, margin: '0 auto 1rem', opacity: 0.5 }}>
                        <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m0 12.75h7.5m-7.5 3H12M10.5 2.25H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z" />
                      </svg>
                      <p>Không tìm thấy khoản khấu trừ nào</p>
                    </div>
                  </td>
                </tr>
              ) : (
                paginatedItems.map(item => (
                  <tr key={item.idKhoanKhauTru}>
                    <td>
                      <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{item.tenKhoanKhauTru}</div>
                    </td>
                    <td>
                      <span className={`kkt-badge ${item.loaiCongThuc === 'Tỷ lệ phần trăm' ? 'kkt-badge-blue' : 'kkt-badge-gray'}`}>
                        {item.loaiCongThuc === 'Tỷ lệ phần trăm' ? '%' : '₫'}
                        {' '}{item.loaiCongThuc}
                      </span>
                    </td>
                    <td>
                      <span className="mono" style={{ color: 'var(--text-primary)', fontWeight: 500 }}>
                        {item.loaiCongThuc === 'Tỷ lệ phần trăm'
                          ? `${item.giaTri}%`
                          : `-${formatCurrency(item.giaTri)} ₫`}
                      </span>
                    </td>
                    <td className="truncate" style={{ color: 'var(--text-secondary)' }}>
                      {item.ghiChu || '—'}
                    </td>
                    <td style={{ textAlign: 'center' }}>
                      <span className={`kkt-badge ${item.isActive ? 'kkt-badge-success' : 'kkt-badge-gray'}`}>
                        {item.isActive ? 'Đang dùng' : 'Tạm dừng'}
                      </span>
                    </td>
                    <td>
                      <div className="kkt-actions">
                        <button
                          className="kkt-btn-actions"
                          onClick={() => setOpenDropdownId(prev => prev === item.idKhoanKhauTru ? null : item.idKhoanKhauTru)}
                        >
                          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" width={20} height={20}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 12.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 18.75a.75.75 0 110-1.5.75.75 0 010 1.5z" />
                          </svg>
                        </button>
                        {openDropdownId === item.idKhoanKhauTru && (
                          <div ref={dropdownRef} className="kkt-actions-dropdown">
                            <button className="kkt-dropdown-item info" onClick={() => openEditModal(item)}>
                              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" width={16} height={16}>
                                <path strokeLinecap="round" strokeLinejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Z" />
                              </svg>
                              Chỉnh sửa
                            </button>
                            <button className="kkt-dropdown-item danger" onClick={() => handleDelete(item)}>
                              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" width={16} height={16}>
                                <path strokeLinecap="round" strokeLinejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
                              </svg>
                              Xóa
                            </button>
                          </div>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {totalPages > 0 && (
          <div className="kkt-pagination" style={{ justifyContent: 'center' }}>
            <div style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
              <button
                className="kkt-btn kkt-btn-secondary"
                disabled={currentPage <= 1 || loading}
                onClick={() => setCurrentPage(p => p - 1)}
                style={{ padding: '0.35rem 0.75rem' }}
              >
                Trước
              </button>
              <span style={{ fontSize: '0.875rem', fontWeight: 500, color: 'var(--text-secondary)' }}>
                Trang <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{currentPage}</span> / <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{totalPages}</span>
              </span>
              <button
                className="kkt-btn kkt-btn-secondary"
                disabled={currentPage >= totalPages || loading}
                onClick={() => setCurrentPage(p => p + 1)}
                style={{ padding: '0.35rem 0.75rem' }}
              >
                Sau
              </button>
            </div>
          </div>
        )}
      </div>

      {/* Modal */}
      {modalOpen && (
        <div className="kkt-modal-overlay" onMouseDown={(e) => { if (e.target === e.currentTarget) closeModal(); }}>
          <div className="kkt-modal" role="dialog" aria-modal="true">
            <div className="kkt-modal-header">
              <h2 className="kkt-modal-title">{editingItem ? 'Chỉnh sửa khoản khấu trừ' : 'Thêm khoản khấu trừ'}</h2>
              <button className="kkt-modal-close" onClick={closeModal} aria-label="Đóng">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12" />
                </svg>
              </button>
            </div>

            <div className="kkt-modal-body">
              {formError && (
                <div style={{ color: 'var(--danger)', fontSize: '0.875rem', marginBottom: '1rem', padding: '0.5rem', background: '#fee2e2', borderRadius: '6px' }}>
                  {formError}
                </div>
              )}

              {/* Tên khoản */}
              <div className="kkt-form-group">
                <label htmlFor="kkt-input-ten" className="kkt-form-label">Tên khoản khấu trừ <span className="required">*</span></label>
                <input
                  id="kkt-input-ten"
                  className="kkt-form-input"
                  type="text"
                  placeholder="VD: BHXH (8%), BHYT (1.5%)"
                  value={form.tenKhoanKhauTru}
                  onChange={e => setForm(f => ({ ...f, tenKhoanKhauTru: e.target.value }))}
                />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                {/* Loại công thức */}
                <div className="kkt-form-group">
                  <label htmlFor="kkt-select-loai" className="kkt-form-label">Cách tính <span className="required">*</span></label>
                  <select
                    id="kkt-select-loai"
                    className="kkt-form-select"
                    value={form.loaiCongThuc}
                    onChange={e => setForm(f => ({ ...f, loaiCongThuc: e.target.value as LoaiCongThuc }))}
                  >
                    {LOAI_CONG_THUC_OPTIONS.map(opt => (
                      <option key={opt.value} value={opt.value}>{opt.label}</option>
                    ))}
                  </select>
                </div>

                {/* Giá trị */}
                <div className="kkt-form-group">
                  <label htmlFor="kkt-input-gia-tri" className="kkt-form-label">
                    Giá trị <span className="required">*</span>
                    <span style={{ fontWeight: 400, color: 'var(--text-secondary)', marginLeft: 4 }}>
                      ({form.loaiCongThuc === 'TY_LE_PHAN_TRAM' ? '%' : 'VNĐ'})
                    </span>
                  </label>
                  <div style={{ position: 'relative' }}>
                    <input
                      id="kkt-input-gia-tri"
                      className="kkt-form-input"
                      style={{ paddingRight: '2rem' }}
                      type="number"
                      min="0"
                      step={form.loaiCongThuc === 'TY_LE_PHAN_TRAM' ? '0.1' : '1000'}
                      placeholder={form.loaiCongThuc === 'TY_LE_PHAN_TRAM' ? 'VD: 8' : 'VD: 500000'}
                      value={form.giaTri}
                      onChange={e => setForm(f => ({ ...f, giaTri: e.target.value }))}
                    />
                    <span style={{ position: 'absolute', right: '0.75rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-secondary)' }}>
                      {form.loaiCongThuc === 'TY_LE_PHAN_TRAM' ? '%' : '₫'}
                    </span>
                  </div>
                </div>
              </div>

              {/* Ghi chú */}
              <div className="kkt-form-group">
                <label htmlFor="kkt-input-ghi-chu" className="kkt-form-label">Ghi chú / Diễn giải công thức</label>
                <textarea
                  id="kkt-input-ghi-chu"
                  className="kkt-form-textarea"
                  placeholder="VD: 8% × 13.000.000 (Mức lương P1)"
                  value={form.ghiChu}
                  onChange={e => setForm(f => ({ ...f, ghiChu: e.target.value }))}
                />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr', gap: '1rem' }}>
                {/* Trạng thái */}
                <div className="kkt-form-group">
                  <label className="kkt-form-label">Trạng thái kích hoạt</label>
                  <label className="kkt-checkbox-wrapper" style={{ marginTop: '0.5rem' }}>
                    <input
                      type="checkbox"
                      checked={form.isActive}
                      onChange={e => setForm(f => ({ ...f, isActive: e.target.checked }))}
                    />
                    <span className="kkt-checkbox-label">
                      {form.isActive ? 'Đang kích hoạt' : 'Tạm dừng'}
                    </span>
                  </label>
                </div>
              </div>
            </div>

            <div className="kkt-modal-footer">
              <button className="kkt-btn kkt-btn-secondary" onClick={closeModal}>
                Hủy
              </button>
              <button
                className="kkt-btn kkt-btn-primary"
                onClick={handleSave}
                disabled={saving}
              >
                {saving ? 'Đang lưu...' : (editingItem ? 'Cập nhật' : 'Thêm mới')}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

