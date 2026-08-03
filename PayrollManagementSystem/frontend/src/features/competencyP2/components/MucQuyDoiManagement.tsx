import React, { useEffect, useState } from 'react';
import { useMucQuyDoi } from '../hooks/useMucQuyDoi';
import { MucQuyDoiDto } from '../types/mucQuyDoi.types';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { Toast } from '../../../components/Toast/Toast';
import { ConfirmModal } from '../../../components/ConfirmModal/ConfirmModal';
import './CompetencyManagement.css';

export const MucQuyDoiManagement: React.FC = () => {
  const { data, loading, fetchQuyDoi, createQuyDoi, updateQuyDoi, deleteQuyDoi, toast, setToast } = useMucQuyDoi();
  
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [editingItem, setEditingItem] = useState<MucQuyDoiDto | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  const [formData, setFormData] = useState({
    xepLoai: '',
    diemToiThieu: '',
    diemToiDa: '',
    heSoP2: ''
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  
  const [activeDropdown, setActiveDropdown] = useState<string | null>(null);

  const [confirmDelete, setConfirmDelete] = useState<MucQuyDoiDto | null>(null);

  const {
    currentData,
    allFilteredAndSortedData,
    currentPage,
    totalPages,
    setCurrentPage,
    sortKey,
    sortDirection,
    handleSort,
    searchTerm,
    setSearchTerm
  } = useDataTable<any>({
    data: data,
    initialPageSize: 10,
    searchableFields: ['xepLoai']
  });

  const handleExportExcel = () => {
    const columns: ExportColumn<any>[] = [
      { header: 'Xếp loại', key: 'xepLoai' },
      { header: 'Điểm tối thiểu', key: 'diemToiThieu' },
      { header: 'Điểm tối đa', key: 'diemToiDa' },
      { header: 'Hệ số P2', key: 'heSoP2' },
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'MucQuyDoi');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<any>[] = [
      { header: 'Xếp loại', key: 'xepLoai' },
      { header: 'Điểm tối thiểu', key: 'diemToiThieu' },
      { header: 'Điểm tối đa', key: 'diemToiDa' },
      { header: 'Hệ số P2', key: 'heSoP2' },
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'MucQuyDoi', 'Mức quy đổi P2');
  };

  useEffect(() => {
    fetchQuyDoi();
    
    const handleClickOutside = (e: MouseEvent) => {
      if (activeDropdown && !(e.target as Element).closest('.cp2-actions')) {
        setActiveDropdown(null);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [fetchQuyDoi, activeDropdown]);

  const handleOpenModal = (record?: MucQuyDoiDto) => {
    setEditingItem(record || null);
    if (record) {
      setFormData({
        xepLoai: record.xepLoai,
        diemToiThieu: record.diemToiThieu.toString(),
        diemToiDa: record.diemToiDa.toString(),
        heSoP2: record.heSoP2.toString()
      });
    } else {
      setFormData({ xepLoai: '', diemToiThieu: '', diemToiDa: '', heSoP2: '' });
    }
    setErrors({});
    setIsModalVisible(true);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors(prev => ({ ...prev, [name]: '' }));
  };

  const validate = () => {
    const newErrors: Record<string, string> = {};
    if (!formData.xepLoai.trim()) newErrors.xepLoai = 'Vui lòng nhập xếp loại';
    if (formData.xepLoai.length > 50) newErrors.xepLoai = 'Không vượt quá 50 ký tự';
    
    if (formData.diemToiThieu === '') newErrors.diemToiThieu = 'Bắt buộc nhập';
    else if (Number(formData.diemToiThieu) < 0) newErrors.diemToiThieu = 'Phải >= 0';
    
    if (formData.diemToiDa === '') newErrors.diemToiDa = 'Bắt buộc nhập';
    else {
      const min = Number(formData.diemToiThieu);
      const max = Number(formData.diemToiDa);
      if (max <= min) newErrors.diemToiDa = 'Điểm tối đa phải lớn hơn điểm tối thiểu';
    }

    if (formData.heSoP2 === '') newErrors.heSoP2 = 'Vui lòng nhập hệ số P2';
    else if (Number(formData.heSoP2) < 0) newErrors.heSoP2 = 'Hệ số phải >= 0';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    setIsSubmitting(true);
    try {
      const payload = {
        xepLoai: formData.xepLoai,
        diemToiThieu: Number(formData.diemToiThieu),
        diemToiDa: Number(formData.diemToiDa),
        heSoP2: Number(formData.heSoP2)
      };

      let success = false;
      if (editingItem) {
        success = await updateQuyDoi(editingItem.idQuyDoi, payload);
      } else {
        success = await createQuyDoi(payload);
      }

      if (success) {
        setIsModalVisible(false);
        fetchQuyDoi();
        setToast({ message: editingItem ? 'Cập nhật thành công!' : 'Thêm mới thành công!', type: 'success' });
      } else {
        setToast({ message: 'Lưu thất bại. Vui lòng kiểm tra lại.', type: 'error' });
      }
    } catch (info) {
      console.log('Error:', info);
      setToast({ message: 'Có lỗi hệ thống xảy ra khi lưu!', type: 'error' });
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteClick = (record: MucQuyDoiDto) => {
    setConfirmDelete(record);
    setActiveDropdown(null);
  };

  const confirmDeleteAction = async () => {
    if (!confirmDelete) return;
    try {
      const success = await deleteQuyDoi(confirmDelete.idQuyDoi);
      if (success) {
        fetchQuyDoi();
        setToast({ message: 'Xóa thành công!', type: 'success' });
      } else {
        setToast({ message: 'Xóa thất bại.', type: 'error' });
      }
    } catch (error) {
      setToast({ message: 'Lỗi khi xóa.', type: 'error' });
    } finally {
      setConfirmDelete(null);
    }
  };

  return (
    <div className="cp2-container">
      <div className="cp2-header">
        <div className="cp2-header-title">
          <h2>⚙️ Cấu Hình Mức Quy Đổi</h2>
          <p>Quản lý các dải điểm đánh giá năng lực và hệ số lương P2 tương ứng</p>
        </div>
        <button 
          className="cp2-btn cp2-btn-primary" 
          onClick={() => handleOpenModal()}
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
          </svg>
          Thêm Mức Quy Đổi
        </button>
      </div>

      <div className="cp2-controls-wrapper">
        <div style={{ display: 'flex', justifyContent: 'space-between', padding: '1rem', borderBottom: '1px solid var(--border-color)', gap: '1rem', flexWrap: 'wrap' }}>
          <div className="cp2-input-wrapper" style={{ flex: 1, minWidth: '250px', position: 'relative' }}>
            <input
              type="text"
              placeholder="Tìm kiếm mức quy đổi..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="cp2-select"
              style={{ width: '100%', paddingLeft: '0.75rem' }}
            />
          </div>
          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
        </div>
        <div className="cp2-table-container custom-scrollbar">
          {loading ? (
            <div className="cp2-loader">
              <div className="cp2-spinner"></div>
            </div>
          ) : data.length > 0 ? (
            <table className="cp2-table">
              <thead>
                <tr>
                  <SortableHeader label="Xếp loại" sortKey="xepLoai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                  <SortableHeader label="Điểm tối thiểu" sortKey="diemToiThieu" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'right' }} />
                  <SortableHeader label="Điểm tối đa" sortKey="diemToiDa" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'right' }} />
                  <SortableHeader label="Hệ số P2" sortKey="heSoP2" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <th style={{ textAlign: 'right' }}>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {currentData.map(record => {
                  let badgeClass = "cp2-badge-gray";
                  if (record.xepLoai.includes('A')) badgeClass = "cp2-badge-success";
                  else if (record.xepLoai.includes('B')) badgeClass = "cp2-badge-blue";
                  else if (record.xepLoai.includes('C')) badgeClass = "cp2-badge-warning";
                  else if (record.xepLoai.includes('D')) badgeClass = "cp2-badge-danger";

                  return (
                    <tr key={record.idQuyDoi}>
                      <td>
                        <span className={`cp2-badge ${badgeClass}`} style={{ fontSize: '0.85rem' }}>
                          {record.xepLoai}
                        </span>
                      </td>
                      <td style={{ textAlign: 'right', fontWeight: 600 }}>
                        {record.diemToiThieu.toLocaleString('vi-VN')}
                      </td>
                      <td style={{ textAlign: 'right', fontWeight: 600, color: 'var(--primary)' }}>
                        {record.diemToiDa.toLocaleString('vi-VN')}
                      </td>
                      <td style={{ textAlign: 'center', fontWeight: 700, color: 'var(--success-text)' }}>
                        {record.heSoP2.toLocaleString('vi-VN')}
                      </td>
                      <td>
                        <div className="cp2-actions">
                          <button 
                            className="cp2-btn-actions"
                            onClick={(e) => {
                              e.stopPropagation();
                              setActiveDropdown(activeDropdown === record.idQuyDoi ? null : record.idQuyDoi);
                            }}
                          >
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                              <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 12.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 18.75a.75.75 0 110-1.5.75.75 0 010 1.5z" />
                            </svg>
                          </button>
                          
                          {activeDropdown === record.idQuyDoi && (
                            <div className="cp2-actions-dropdown">
                              <button 
                                className="cp2-dropdown-item" 
                                onClick={() => {
                                  handleOpenModal(record);
                                  setActiveDropdown(null);
                                }}
                              >
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                  <path strokeLinecap="round" strokeLinejoin="round" d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L6.832 19.82a4.5 4.5 0 01-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 011.13-1.897L16.863 4.487zm0 0L19.5 7.125" />
                                </svg>
                                Sửa
                              </button>
                              <button 
                                className="cp2-dropdown-item danger" 
                                onClick={() => {
                                  handleDeleteClick(record);
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
                  );
                })}
              </tbody>
            </table>
          ) : (
            <div className="cp2-empty">
              <p>Chưa có mức quy đổi nào được cấu hình.</p>
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
              <h3 className="cp2-modal-title">
                {editingItem ? "Cập Nhật Mức Quy Đổi" : "Thêm Mới Mức Quy Đổi"}
              </h3>
              <button className="cp2-modal-close" onClick={() => setIsModalVisible(false)} disabled={isSubmitting}>
                &times;
              </button>
            </div>

            <div className="cp2-modal-body">
              <form id="quydoi-form" onSubmit={handleSubmit}>
                <div className="cp2-form-group">
                  <label className="cp2-form-label">Xếp loại (VD: A+, A, B) <span className="required">*</span></label>
                  <input
                    type="text"
                    name="xepLoai"
                    value={formData.xepLoai}
                    onChange={handleInputChange}
                    className="cp2-form-input"
                    placeholder="Nhập tên xếp loại..."
                  />
                  {errors.xepLoai && <span className="cp2-form-error">{errors.xepLoai}</span>}
                </div>
                
                <div style={{ display: 'flex', gap: '1rem' }}>
                  <div className="cp2-form-group" style={{ flex: 1 }}>
                    <label className="cp2-form-label">Điểm tối thiểu <span className="required">*</span></label>
                    <input
                      type="number"
                      step="0.1"
                      name="diemToiThieu"
                      value={formData.diemToiThieu}
                      onChange={handleInputChange}
                      className="cp2-form-input"
                      placeholder="0.0"
                    />
                    {errors.diemToiThieu && <span className="cp2-form-error">{errors.diemToiThieu}</span>}
                  </div>
                  
                  <div className="cp2-form-group" style={{ flex: 1 }}>
                    <label className="cp2-form-label">Điểm tối đa <span className="required">*</span></label>
                    <input
                      type="number"
                      step="0.1"
                      name="diemToiDa"
                      value={formData.diemToiDa}
                      onChange={handleInputChange}
                      className="cp2-form-input"
                      placeholder="10.0"
                    />
                    {errors.diemToiDa && <span className="cp2-form-error">{errors.diemToiDa}</span>}
                  </div>
                </div>

                <div className="cp2-form-group">
                  <label className="cp2-form-label">Hệ số P2 <span className="required">*</span></label>
                  <input
                    type="number"
                    step="0.01"
                    name="heSoP2"
                    value={formData.heSoP2}
                    onChange={handleInputChange}
                    className="cp2-form-input"
                    placeholder="1.0"
                  />
                  {errors.heSoP2 && <span className="cp2-form-error">{errors.heSoP2}</span>}
                </div>
              </form>
            </div>

            <div className="cp2-modal-footer">
              <button type="button" className="cp2-btn cp2-btn-secondary" onClick={() => setIsModalVisible(false)} disabled={isSubmitting}>
                Hủy bỏ
              </button>
              <button type="submit" form="quydoi-form" className="cp2-btn cp2-btn-primary" disabled={isSubmitting}>
                {isSubmitting ? 'Đang lưu...' : 'Lưu lại'}
              </button>
            </div>
          </div>
        </div>
      )}

      {toast && <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />}
      
      <ConfirmModal 
        isOpen={!!confirmDelete} 
        title="Xác nhận xóa" 
        message={`Bạn có chắc chắn muốn xóa xếp loại "${confirmDelete?.xepLoai}" không? thao tác này không thể hoàn tác.`} 
        onConfirm={confirmDeleteAction} 
        onCancel={() => setConfirmDelete(null)} 
      />
    </div>
  );
};
