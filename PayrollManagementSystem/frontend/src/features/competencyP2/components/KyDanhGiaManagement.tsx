import React, { useEffect, useState } from 'react';
import { useKyDanhGia } from '../hooks/useKyDanhGia';
import { kyDanhGiaApi } from '../api/kyDanhGiaApi';
import './CompetencyManagement.css';

export const KyDanhGiaManagement: React.FC = () => {
  const { data, loading, fetchKyDanhGia, createKyDanhGia } = useKyDanhGia();
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState({
    tenKyDanhGia: '',
    ngayBatDau: '',
    ngayKetThuc: ''
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  
  const [activeDropdown, setActiveDropdown] = useState<string | null>(null);

  // Pagination states
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;
  const totalItems = data.length;
  const totalPages = Math.ceil(totalItems / pageSize);
  const currentData = data.slice((currentPage - 1) * pageSize, currentPage * pageSize);

  useEffect(() => {
    fetchKyDanhGia();
    
    const handleClickOutside = (e: MouseEvent) => {
      if (activeDropdown && !(e.target as Element).closest('.cp2-actions')) {
        setActiveDropdown(null);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [fetchKyDanhGia, activeDropdown]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors(prev => ({ ...prev, [name]: '' }));
  };

  const validate = () => {
    const newErrors: Record<string, string> = {};
    if (!formData.tenKyDanhGia.trim()) newErrors.tenKyDanhGia = 'Vui lòng nhập tên kỳ đánh giá';
    if (!formData.ngayBatDau) newErrors.ngayBatDau = 'Vui lòng chọn ngày bắt đầu';
    if (!formData.ngayKetThuc) newErrors.ngayKetThuc = 'Vui lòng chọn ngày kết thúc';
    
    if (formData.ngayBatDau && formData.ngayKetThuc && new Date(formData.ngayBatDau) > new Date(formData.ngayKetThuc)) {
      newErrors.ngayKetThuc = 'Ngày kết thúc phải lớn hơn ngày bắt đầu';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    
    setIsSubmitting(true);
    const success = await createKyDanhGia(formData);
    setIsSubmitting(false);
    
    if (success) {
      setIsModalVisible(false);
      setFormData({ tenKyDanhGia: '', ngayBatDau: '', ngayKetThuc: '' });
      fetchKyDanhGia();
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm("Bạn có chắc chắn muốn xóa kỳ đánh giá này không?")) {
      try {
        const res = await kyDanhGiaApi.delete(id);
        if (res.succeeded) {
          fetchKyDanhGia();
        }
      } catch (e: any) {
        alert(e.response?.data?.Message || 'Xóa thất bại');
      }
    }
  };

  const handleChangeStatus = async (id: string, status: number, force: boolean = false) => {
    try {
      const res = await kyDanhGiaApi.changeStatus(id, status, force);
      if (res.succeeded) {
        fetchKyDanhGia();
      }
    } catch (e: any) {
      const errorMsg = e.response?.data?.Message;
      if (errorMsg === "HienTaiCoPhieuChuaXong") {
        if (window.confirm('Hiện tại có phiếu đánh giá chưa hoàn thành. Bạn có chắc chắn muốn ép chốt kỳ đánh giá này không? (Hệ số P2 sẽ chỉ được cập nhật cho các phiếu đã hoàn thành).')) {
          handleChangeStatus(id, status, true);
        }
      } else {
        alert(errorMsg || 'Cập nhật thất bại');
      }
    }
  };

  return (
    <div className="cp2-container">
      <div className="cp2-header">
        <div className="cp2-header-title">
          <h2>Quản lý Kỳ đánh giá Năng lực</h2>
          <p>Tạo và quản lý các kỳ đánh giá P2 định kỳ</p>
        </div>
        <button 
          className="cp2-btn cp2-btn-primary" 
          onClick={() => {
            setFormData({ tenKyDanhGia: '', ngayBatDau: '', ngayKetThuc: '' });
            setErrors({});
            setIsModalVisible(true);
          }}
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
          </svg>
          Tạo kỳ đánh giá
        </button>
      </div>

      <div className="cp2-controls-wrapper">
        <div className="cp2-table-container custom-scrollbar">
          {loading ? (
            <div className="cp2-loader">
              <div className="cp2-spinner"></div>
            </div>
          ) : currentData.length > 0 ? (
            <table className="cp2-table">
              <thead>
                <tr>
                  <th>Tên kỳ đánh giá</th>
                  <th>Ngày bắt đầu</th>
                  <th>Ngày kết thúc</th>
                  <th style={{ textAlign: 'center' }}>Trạng thái</th>
                  <th style={{ textAlign: 'right' }}>Hành động</th>
                </tr>
              </thead>
              <tbody>
                {currentData.map(record => {
                  let badgeClass = "cp2-badge-gray";
                  if (record.trangThai === 'DANG_DANH_GIA') badgeClass = "cp2-badge-success";
                  if (record.trangThai === 'KHOI_TAO') badgeClass = "cp2-badge-blue";
                  if (record.trangThai === 'DA_CHOT') badgeClass = "cp2-badge-warning";
                  if (record.trangThai === 'DA_HUY') badgeClass = "cp2-badge-danger";

                  return (
                    <tr key={record.idKyDanhGia}>
                      <td style={{ fontWeight: 600, color: '#111827' }}>{record.tenKyDanhGia}</td>
                      <td>{record.ngayBatDau}</td>
                      <td>{record.ngayKetThuc}</td>
                      <td style={{ textAlign: 'center' }}>
                        <span className={`cp2-badge ${badgeClass}`}>
                          {record.tenTrangThai || record.trangThai}
                        </span>
                      </td>
                      <td>
                        <div className="cp2-actions">
                          <button 
                            className="cp2-btn-actions"
                            onClick={(e) => {
                              e.stopPropagation();
                              setActiveDropdown(activeDropdown === record.idKyDanhGia ? null : record.idKyDanhGia);
                            }}
                          >
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                              <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 12.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 18.75a.75.75 0 110-1.5.75.75 0 010 1.5z" />
                            </svg>
                          </button>
                          
                          {activeDropdown === record.idKyDanhGia && (
                            <div className="cp2-actions-dropdown">
                              {record.trangThai === 'KHOI_TAO' && (
                                <>
                                  <button 
                                    className="cp2-dropdown-item success" 
                                    onClick={() => {
                                      if (window.confirm("Mở kỳ đánh giá này? Nhân viên sẽ bắt đầu có thể tự đánh giá.")) {
                                        handleChangeStatus(record.idKyDanhGia, 1);
                                      }
                                      setActiveDropdown(null);
                                    }}
                                  >
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                      <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 10.5V6.75a4.5 4.5 0 119 0v3.75M3.75 21.75h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H3.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z" />
                                    </svg>
                                    Mở đánh giá
                                  </button>
                                  <button 
                                    className="cp2-dropdown-item danger" 
                                    onClick={() => {
                                      handleDelete(record.idKyDanhGia);
                                      setActiveDropdown(null);
                                    }}
                                  >
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                      <path strokeLinecap="round" strokeLinejoin="round" d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0" />
                                    </svg>
                                    Xóa kỳ đánh giá
                                  </button>
                                </>
                              )}
                              
                              {record.trangThai === 'DANG_DANH_GIA' && (
                                <>
                                  <button 
                                    className="cp2-dropdown-item warning" 
                                    onClick={() => {
                                      if (window.confirm("Bạn có chắc muốn chốt kỳ đánh giá? Sau khi chốt, hệ số P2 sẽ được tự động cập nhật vào thông tin nhân sự.")) {
                                        handleChangeStatus(record.idKyDanhGia, 2);
                                      }
                                      setActiveDropdown(null);
                                    }}
                                  >
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                      <path strokeLinecap="round" strokeLinejoin="round" d="M16.5 10.5V6.75a4.5 4.5 0 10-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H6.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z" />
                                    </svg>
                                    Chốt kỳ
                                  </button>
                                  <button 
                                    className="cp2-dropdown-item danger" 
                                    onClick={() => {
                                      if (window.confirm("Hủy kỳ đánh giá này? Tất cả phiếu sẽ bị hủy.")) {
                                        handleChangeStatus(record.idKyDanhGia, 3);
                                      }
                                      setActiveDropdown(null);
                                    }}
                                  >
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                      <path strokeLinecap="round" strokeLinejoin="round" d="M9.75 9.75l4.5 4.5m0-4.5l-4.5 4.5M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                    Hủy kỳ
                                  </button>
                                </>
                              )}
                              
                              {record.trangThai !== 'KHOI_TAO' && record.trangThai !== 'DANG_DANH_GIA' && (
                                <div style={{ padding: '0.5rem', color: '#9ca3af', fontSize: '0.85rem', textAlign: 'center' }}>
                                  Không có hành động
                                </div>
                              )}
                            </div>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          ) : (
            <div className="cp2-empty">
              <p>Chưa có kỳ đánh giá nào.</p>
            </div>
          )}
        </div>

        {totalPages > 0 && (
          <div className="cp2-pagination">
            <button 
              className="cp2-btn cp2-btn-secondary" 
              onClick={() => setCurrentPage(p => p - 1)} 
              disabled={currentPage === 1 || loading}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              Trước
            </button>
            <div className="cp2-pagination-info">
              Trang <span>{currentPage}</span> / <span>{totalPages}</span>
            </div>
            <button 
              className="cp2-btn cp2-btn-secondary" 
              onClick={() => setCurrentPage(p => p + 1)} 
              disabled={currentPage === totalPages || loading}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              Sau
            </button>
          </div>
        )}
      </div>

      {isModalVisible && (
        <div className="cp2-modal-overlay">
          <div className="cp2-modal">
            <div className="cp2-modal-header">
              <h3 className="cp2-modal-title">Tạo Kỳ Đánh Giá Mới</h3>
              <button className="cp2-modal-close" onClick={() => setIsModalVisible(false)} disabled={isSubmitting}>
                &times;
              </button>
            </div>

            <div className="cp2-modal-body">
              <form id="ky-form" onSubmit={handleAdd}>
                <div className="cp2-form-group">
                  <label className="cp2-form-label">Tên kỳ đánh giá <span className="required">*</span></label>
                  <input
                    type="text"
                    name="tenKyDanhGia"
                    value={formData.tenKyDanhGia}
                    onChange={handleInputChange}
                    className="cp2-form-input"
                    placeholder="VD: Đánh giá năng lực cuối năm 2026"
                  />
                  {errors.tenKyDanhGia && <span className="cp2-form-error">{errors.tenKyDanhGia}</span>}
                </div>

                <div className="cp2-form-group">
                  <label className="cp2-form-label">Ngày bắt đầu <span className="required">*</span></label>
                  <input
                    type="date"
                    name="ngayBatDau"
                    value={formData.ngayBatDau}
                    onChange={handleInputChange}
                    className="cp2-form-input"
                  />
                  {errors.ngayBatDau && <span className="cp2-form-error">{errors.ngayBatDau}</span>}
                </div>

                <div className="cp2-form-group">
                  <label className="cp2-form-label">Ngày kết thúc <span className="required">*</span></label>
                  <input
                    type="date"
                    name="ngayKetThuc"
                    value={formData.ngayKetThuc}
                    onChange={handleInputChange}
                    className="cp2-form-input"
                  />
                  {errors.ngayKetThuc && <span className="cp2-form-error">{errors.ngayKetThuc}</span>}
                </div>
              </form>
            </div>

            <div className="cp2-modal-footer">
              <button type="button" className="cp2-btn cp2-btn-secondary" onClick={() => setIsModalVisible(false)} disabled={isSubmitting}>
                Hủy bỏ
              </button>
              <button type="submit" form="ky-form" className="cp2-btn cp2-btn-primary" disabled={isSubmitting}>
                {isSubmitting ? 'Đang tạo...' : 'Tạo kỳ'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
