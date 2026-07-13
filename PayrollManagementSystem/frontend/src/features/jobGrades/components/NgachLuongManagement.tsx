import React, { useEffect, useState } from 'react';
import { useJobGrades } from '../hooks/useJobGrades';
import { JobGrade } from '../types/jobGrade.types';
import { JobGradeSalaryStepDrawer } from './JobGradeSalaryStepDrawer';
import './JobGradeManagement.css';

export const NgachLuongManagement: React.FC = () => {
  const { jobGrades, loading, fetchJobGrades, createJobGrade, updateJobGrade, deleteJobGrade } = useJobGrades();

  useEffect(() => {
    fetchJobGrades();
  }, [fetchJobGrades]);

  const [isModalVisible, setIsModalVisible] = useState(false);
  const [editingGrade, setEditingGrade] = useState<JobGrade | null>(null);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [formData, setFormData] = useState({
    tenNgachLuong: '',
    moTa: '',
    trangThai: 1
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selectedGradeId, setSelectedGradeId] = useState<string | null>(null);
  const [selectedGradeName, setSelectedGradeName] = useState<string>('');

  const [activeDropdown, setActiveDropdown] = useState<string | null>(null);

  // Pagination states
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;
  const totalItems = jobGrades.length;
  const totalPages = Math.ceil(totalItems / pageSize);
  const currentData = jobGrades.slice((currentPage - 1) * pageSize, currentPage * pageSize);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (activeDropdown && !(e.target as Element).closest('.jg-actions')) {
        setActiveDropdown(null);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [activeDropdown]);

  const handleAdd = () => {
    setEditingGrade(null);
    setFormData({ tenNgachLuong: '', moTa: '', trangThai: 1 });
    setErrors({});
    setIsModalVisible(true);
  };

  const handleEdit = (record: JobGrade) => {
    setEditingGrade(record);
    setFormData({
      tenNgachLuong: record.tenNgachLuong,
      moTa: record.moTa || '',
      trangThai: record.trangThai
    });
    setErrors({});
    setIsModalVisible(true);
  };

  const handleDelete = async (id: string, name: string) => {
    if (window.confirm(`Bạn có chắc chắn muốn xóa ngạch lương "${name}" không?`)) {
      const success = await deleteJobGrade(id);
      if (success) {
        fetchJobGrades();
      }
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: name === 'trangThai' ? Number(value) : value }));
    if (errors[name]) setErrors(prev => ({ ...prev, [name]: '' }));
  };

  const validate = () => {
    const newErrors: Record<string, string> = {};
    if (!formData.tenNgachLuong.trim()) newErrors.tenNgachLuong = 'Vui lòng nhập tên ngạch lương';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleModalOk = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    
    try {
      setSubmitLoading(true);
      
      let success = false;
      if (editingGrade) {
        success = await updateJobGrade({
          idNgachLuong: editingGrade.idNgachLuong,
          ...formData
        });
      } else {
        success = await createJobGrade(formData);
      }
      
      if (success) {
        setIsModalVisible(false);
        fetchJobGrades();
      }
    } catch (error) {
      console.error('Submit error:', error);
    } finally {
      setSubmitLoading(false);
    }
  };

  const openDrawer = (record: JobGrade) => {
    setSelectedGradeId(record.idNgachLuong);
    setSelectedGradeName(record.tenNgachLuong);
    setDrawerOpen(true);
  };

  return (
    <div className="jg-container">
      <div className="jg-header">
        <div className="jg-header-title">
          <h2>Danh Mục Ngạch Lương</h2>
          <p>Quản lý các ngạch lương và bậc lương tương ứng</p>
        </div>
        <button
          className="jg-btn jg-btn-primary"
          onClick={handleAdd}
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
          </svg>
          Thêm Ngạch Lương
        </button>
      </div>

      <div className="jg-controls-wrapper">
        <div className="jg-table-container custom-scrollbar">
          {loading ? (
            <div className="jg-loader">
              <div className="jg-spinner"></div>
            </div>
          ) : currentData.length > 0 ? (
            <table className="jg-table">
              <thead>
                <tr>
                  <th>Mã Ngạch</th>
                  <th>Tên Ngạch</th>
                  <th>Mô Tả</th>
                  <th style={{ textAlign: 'center' }}>Trạng Thái</th>
                  <th style={{ textAlign: 'right' }}>Thao Tác</th>
                </tr>
              </thead>
              <tbody>
                {currentData.map(record => (
                  <tr key={record.idNgachLuong}>
                    <td className="mono">{record.idNgachLuong}</td>
                    <td style={{ fontWeight: 600, color: '#111827' }}>{record.tenNgachLuong}</td>
                    <td>{record.moTa || '-'}</td>
                    <td style={{ textAlign: 'center' }}>
                      {record.trangThai === 1 ? (
                        <span className="jg-badge jg-badge-success">{record.tenTrangThai}</span>
                      ) : (
                        <span className="jg-badge jg-badge-gray">{record.tenTrangThai}</span>
                      )}
                    </td>
                    <td>
                      <div className="jg-actions">
                        <button 
                          className="jg-btn-actions"
                          onClick={(e) => {
                            e.stopPropagation();
                            setActiveDropdown(activeDropdown === record.idNgachLuong ? null : record.idNgachLuong);
                          }}
                        >
                          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 12.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 18.75a.75.75 0 110-1.5.75.75 0 010 1.5z" />
                          </svg>
                        </button>
                        
                        {activeDropdown === record.idNgachLuong && (
                          <div className="jg-actions-dropdown">
                            <button 
                              className="jg-dropdown-item info" 
                              onClick={() => {
                                openDrawer(record);
                                setActiveDropdown(null);
                              }}
                            >
                              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                <path strokeLinecap="round" strokeLinejoin="round" d="M10.5 6h9.75M10.5 6a1.5 1.5 0 11-3 0m3 0a1.5 1.5 0 10-3 0M3.75 6H7.5m3 12h9.75m-9.75 0a1.5 1.5 0 01-3 0m3 0a1.5 1.5 0 00-3 0m-3.75 0H7.5m9-6h3.75m-3.75 0a1.5 1.5 0 01-3 0m3 0a1.5 1.5 0 00-3 0m-9.75 0h9.75" />
                              </svg>
                              Bậc lương
                            </button>
                            <button 
                              className="jg-dropdown-item" 
                              onClick={() => {
                                handleEdit(record);
                                setActiveDropdown(null);
                              }}
                            >
                              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                <path strokeLinecap="round" strokeLinejoin="round" d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L6.832 19.82a4.5 4.5 0 01-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 011.13-1.897L16.863 4.487zm0 0L19.5 7.125" />
                              </svg>
                              Sửa
                            </button>
                            <button 
                              className="jg-dropdown-item danger" 
                              onClick={() => {
                                handleDelete(record.idNgachLuong, record.tenNgachLuong);
                                setActiveDropdown(null);
                              }}
                            >
                              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                <path strokeLinecap="round" strokeLinejoin="round" d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0" />
                              </svg>
                              Xóa
                            </button>
                          </div>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="jg-empty">
              <p>Không có dữ liệu ngạch lương.</p>
            </div>
          )}
        </div>

        {totalPages > 0 && (
          <div className="jg-pagination">
            <button 
              className="jg-btn jg-btn-secondary" 
              onClick={() => setCurrentPage(p => p - 1)} 
              disabled={currentPage === 1 || loading}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              Trước
            </button>
            <div className="jg-pagination-info">
              Trang <span>{currentPage}</span> / <span>{totalPages}</span>
            </div>
            <button 
              className="jg-btn jg-btn-secondary" 
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
        <div className="jg-modal-overlay">
          <div className="jg-modal">
            <div className="jg-modal-header">
              <h3 className="jg-modal-title">
                {editingGrade ? 'Chỉnh Sửa Ngạch Lương' : 'Thêm Mới Ngạch Lương'}
              </h3>
              <button className="jg-modal-close" onClick={() => setIsModalVisible(false)} disabled={submitLoading}>
                &times;
              </button>
            </div>

            <div className="jg-modal-body">
              <form id="jg-form" onSubmit={handleModalOk}>
                <div className="jg-form-group">
                  <label className="jg-form-label">
                    Tên Ngạch Lương <span className="required">*</span>
                  </label>
                  <input
                    type="text"
                    name="tenNgachLuong"
                    value={formData.tenNgachLuong}
                    onChange={handleInputChange}
                    className="jg-form-input"
                    placeholder="VD: G1, G2, Chuyên viên chính..."
                  />
                  {errors.tenNgachLuong && <span className="jg-form-error">{errors.tenNgachLuong}</span>}
                </div>
                
                <div className="jg-form-group">
                  <label className="jg-form-label">Mô tả</label>
                  <textarea
                    name="moTa"
                    value={formData.moTa}
                    onChange={handleInputChange}
                    className="jg-form-textarea"
                    placeholder="Mô tả chi tiết về ngạch lương này"
                  />
                </div>

                {editingGrade && (
                  <div className="jg-form-group">
                    <label className="jg-form-label">Trạng thái</label>
                    <select
                      name="trangThai"
                      value={formData.trangThai}
                      onChange={handleInputChange}
                      className="jg-form-select"
                    >
                      <option value={1}>Đang sử dụng</option>
                      <option value={0}>Ngừng sử dụng</option>
                    </select>
                  </div>
                )}
              </form>
            </div>

            <div className="jg-modal-footer">
              <button type="button" className="jg-btn jg-btn-secondary" onClick={() => setIsModalVisible(false)} disabled={submitLoading}>
                Hủy bỏ
              </button>
              <button type="submit" form="jg-form" className="jg-btn jg-btn-primary" disabled={submitLoading}>
                {submitLoading ? 'Đang lưu...' : 'Lưu lại'}
              </button>
            </div>
          </div>
        </div>
      )}

      <JobGradeSalaryStepDrawer
        jobGradeId={selectedGradeId}
        jobGradeName={selectedGradeName}
        isOpen={drawerOpen}
        onClose={() => setDrawerOpen(false)}
      />
    </div>
  );
};
