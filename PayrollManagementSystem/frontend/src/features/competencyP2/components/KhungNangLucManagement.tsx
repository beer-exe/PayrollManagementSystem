import React, { useEffect, useState } from 'react';
import { useKhungNangLuc } from '../hooks/useKhungNangLuc';
import { positionApi } from '@/features/positions/api/positionApi';
import { PositionDto } from '@/features/positions/types/position.types';
import './CompetencyManagement.css';

// Array of vibrant colors for the donut chart slices
const CHART_COLORS = [
  '#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', 
  '#ec4899', '#06b6d4', '#84cc16', '#f97316', '#6366f1'
];

interface CriteriaFormItem {
  idTieuChi?: string;
  tenNangLuc: string;
  moTa: string;
  tyTrong: string | number; // 0-100
  key: string; // for React list rendering
}

export const KhungNangLucManagement: React.FC = () => {
  const [positions, setPositions] = useState<PositionDto[]>([]);
  const [selectedChucVu, setSelectedChucVu] = useState<string>('');
  
  const { data, loading, fetchByChucVu, createCriteria, updateCriteria, deleteCriteria } = useKhungNangLuc();
  
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  // Dynamic Form State
  const [criteriaList, setCriteriaList] = useState<CriteriaFormItem[]>([]);
  const [activeDropdown, setActiveDropdown] = useState<string | null>(null);

  // Pagination states
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;
  const totalItems = data.length;
  const totalPages = Math.ceil(totalItems / pageSize);
  const currentData = data.slice((currentPage - 1) * pageSize, currentPage * pageSize);

  const totalWeightPercent = criteriaList.reduce((sum: number, item) => sum + (Number(item.tyTrong) || 0), 0);
  const isOverweight = totalWeightPercent > 100;

  const fetchPositions = async () => {
    try {
      const res = await positionApi.getPositions();
      if (res.succeeded) {
        setPositions(res.data);
      }
    } catch (error) {
      console.error("Lỗi khi tải danh sách chức vụ", error);
    }
  };

  useEffect(() => {
    fetchPositions();
    
    const handleClickOutside = (e: MouseEvent) => {
      if (activeDropdown && !(e.target as Element).closest('.cp2-actions')) {
        setActiveDropdown(null);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [activeDropdown]);

  useEffect(() => {
    if (selectedChucVu) {
      fetchByChucVu(selectedChucVu);
      setCurrentPage(1);
    }
  }, [selectedChucVu, fetchByChucVu]);

  const handleOpenConfig = () => {
    // Map existing data to form state, converting tyTrong from 0-1 to 0-100
    const initialValues = data.map(item => ({
      idTieuChi: item.idTieuChi,
      tenNangLuc: item.tenNangLuc,
      moTa: item.moTa || '',
      tyTrong: Number((item.tyTrong * 100).toFixed(1)),
      key: Math.random().toString(36).substring(7)
    }));
    
    setCriteriaList(initialValues);
    setIsModalVisible(true);
  };

  const addCriteria = () => {
    setCriteriaList([
      ...criteriaList, 
      { tenNangLuc: '', moTa: '', tyTrong: '', key: Math.random().toString(36).substring(7) }
    ]);
  };

  const removeCriteria = (key: string) => {
    setCriteriaList(criteriaList.filter(c => c.key !== key));
  };

  const handleCriteriaChange = (key: string, field: string, value: string | number) => {
    setCriteriaList(criteriaList.map(c => c.key === key ? { ...c, [field]: value } : c));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedChucVu) return;
    
    // Validation
    if (totalWeightPercent > 100) {
      alert("Tổng tỷ trọng không được vượt quá 100%");
      return;
    }
    
    const hasEmptyName = criteriaList.some(c => !c.tenNangLuc.trim());
    if (hasEmptyName) {
      alert("Vui lòng nhập đầy đủ tên tiêu chí năng lực.");
      return;
    }
    
    const hasInvalidWeight = criteriaList.some(c => Number(c.tyTrong) <= 0);
    if (hasInvalidWeight) {
      alert("Tỷ trọng phải lớn hơn 0%.");
      return;
    }

    setIsSubmitting(true);
    
    // Find what to create, update, delete
    const existingIdsInForm = criteriaList.map(c => c.idTieuChi).filter(Boolean);
    const deletedItems = data.filter(d => !existingIdsInForm.includes(d.idTieuChi));
    
    try {
      // 1. Delete removed items
      const deletePromises = deletedItems.map(d => deleteCriteria(d.idTieuChi));
      
      // 2. Update existing items
      const updatePromises = criteriaList
        .filter(c => c.idTieuChi)
        .map(c => updateCriteria(c.idTieuChi!, {
          idTieuChi: c.idTieuChi,
          tenNangLuc: c.tenNangLuc,
          moTa: c.moTa || null,
          tyTrong: Number(c.tyTrong) / 100
        }));
        
      // 3. Create new items
      const createPromises = criteriaList
        .filter(c => !c.idTieuChi)
        .map(c => createCriteria({
          idChucVu: selectedChucVu,
          tenNangLuc: c.tenNangLuc,
          moTa: c.moTa || null,
          tyTrong: Number(c.tyTrong) / 100
        }));

      await Promise.all([...deletePromises, ...updatePromises, ...createPromises]);
      setIsModalVisible(false);
      fetchByChucVu(selectedChucVu);
    } catch (error) {
      console.error("Lỗi khi lưu cấu hình:", error);
      alert("Có lỗi xảy ra khi lưu cấu hình");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm("Bạn có chắc muốn xóa tiêu chí này?")) {
      const success = await deleteCriteria(id);
      if (success && selectedChucVu) fetchByChucVu(selectedChucVu);
    }
  };

  // Generate CSS background for conic-gradient pie chart
  const gradientResult = criteriaList.reduce((acc, item, index) => {
    const p = Number(item.tyTrong) || 0;
    if (p <= 0) return acc;
    const start = acc.cumulative;
    const newCumulative = start + p;
    const color = CHART_COLORS[index % CHART_COLORS.length];
    acc.stops.push(`${color} ${start}%, ${color} ${newCumulative}%`);
    acc.cumulative = newCumulative;
    return acc;
  }, { cumulative: 0, stops: [] as string[] });

  const gradientStops = gradientResult.stops;
  if (gradientResult.cumulative < 100) {
    gradientStops.push(`#e5e7eb ${gradientResult.cumulative}%, #e5e7eb 100%`);
  }

  const conicGradient = gradientStops.length > 0 
    ? `conic-gradient(${gradientStops.join(', ')})`
    : 'conic-gradient(#e5e7eb 0 100%)';

  return (
    <div className="cp2-container">
      <div className="cp2-header">
        <div className="cp2-header-title">
          <h2>Cấu hình Khung Năng Lực (P2)</h2>
          <p>Thiết lập các tiêu chí năng lực cốt lõi cho từng chức vụ</p>
        </div>
        <button 
          className="cp2-btn cp2-btn-primary" 
          disabled={!selectedChucVu} 
          onClick={handleOpenConfig}
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M10.34 15.84c-.688-.06-1.386-.09-2.09-.09H7.5a4.5 4.5 0 110-9h.75c.704 0 1.402-.03 2.09-.09m0 9.18c.253.962.584 1.892.985 2.783.247.55.06 1.21-.463 1.511l-.657.38c-.551.318-1.26.117-1.527-.461a20.845 20.845 0 01-1.44-4.282m3.102.069a18.03 18.03 0 01-.59-4.59c0-1.586.205-3.124.59-4.59m0 9.18a23.848 23.848 0 018.835 2.535M10.34 6.66a23.847 23.847 0 008.835-2.535m0 0A23.74 23.74 0 0018.795 3m.38 1.125a23.91 23.91 0 011.014 5.395m-1.014-8.81c-2.28 1.09-4.793 1.77-7.443 1.94m0 0A23.74 23.74 0 0012 3m0 0h-.75m0 0c-.219.03-.436.062-.656.096m0 0A20.845 20.845 0 019.262 5.09" />
          </svg>
          Cấu hình tiêu chí
        </button>
      </div>

      <div className="cp2-controls-wrapper">
        <div className="cp2-filters" style={{ borderBottom: 'none' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', width: '100%', maxWidth: '400px' }}>
            <span style={{ fontWeight: 600, color: '#374151', whiteSpace: 'nowrap' }}>Chọn Chức vụ:</span>
            <select
              value={selectedChucVu}
              onChange={(e) => setSelectedChucVu(e.target.value)}
              className="cp2-select"
            >
              <option value="">-- Chọn chức vụ cần cấu hình --</option>
              {positions.map(p => (
                <option key={p.idChucVu} value={p.idChucVu}>{p.tenChucVu}</option>
              ))}
            </select>
          </div>
        </div>

        {!selectedChucVu ? (
          <div className="cp2-empty">
            <p>Vui lòng chọn một chức vụ để xem và cấu hình khung năng lực.</p>
          </div>
        ) : (
          <>
            <div className="cp2-table-container custom-scrollbar" style={{ borderTop: '1px solid #f3f4f6' }}>
              {loading ? (
                <div className="cp2-loader">
                  <div className="cp2-spinner"></div>
                </div>
              ) : currentData.length > 0 ? (
                <table className="cp2-table">
                  <thead>
                    <tr>
                      <th style={{ width: '35%' }}>Tên năng lực</th>
                      <th style={{ width: '40%' }}>Mô tả</th>
                      <th style={{ textAlign: 'center', width: '15%' }}>Tỷ trọng</th>
                      <th style={{ textAlign: 'right', width: '10%' }}>Hành động</th>
                    </tr>
                  </thead>
                  <tbody>
                    {currentData.map(record => (
                      <tr key={record.idTieuChi}>
                        <td style={{ fontWeight: 600, color: '#111827' }}>{record.tenNangLuc}</td>
                        <td>{record.moTa || '-'}</td>
                        <td style={{ textAlign: 'center' }}>
                          <span className="cp2-badge cp2-badge-blue">
                            {Number((record.tyTrong * 100).toFixed(1))}%
                          </span>
                        </td>
                        <td>
                          <div className="cp2-actions">
                            <button 
                              className="cp2-btn-actions"
                              onClick={(e) => {
                                e.stopPropagation();
                                setActiveDropdown(activeDropdown === record.idTieuChi ? null : record.idTieuChi);
                              }}
                            >
                              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                                <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 12.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 18.75a.75.75 0 110-1.5.75.75 0 010 1.5z" />
                              </svg>
                            </button>
                            
                            {activeDropdown === record.idTieuChi && (
                              <div className="cp2-actions-dropdown">
                                <button 
                                  className="cp2-dropdown-item" 
                                  onClick={() => {
                                    handleOpenConfig();
                                    setActiveDropdown(null);
                                  }}
                                >
                                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L6.832 19.82a4.5 4.5 0 01-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 011.13-1.897L16.863 4.487zm0 0L19.5 7.125" />
                                  </svg>
                                  Sửa tiêu chí
                                </button>
                                <button 
                                  className="cp2-dropdown-item danger" 
                                  onClick={() => {
                                    handleDelete(record.idTieuChi);
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
                <div className="cp2-empty">
                  <p>Không có tiêu chí năng lực nào được cấu hình cho chức vụ này.</p>
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
                  [Sau]
                </button>
              </div>
            )}
          </>
        )}
      </div>

      {isModalVisible && (
        <div className="cp2-modal-overlay">
          <div className="cp2-modal large">
            <div className="cp2-modal-header">
              <h3 className="cp2-modal-title">Cấu Hình Tiêu Chí Năng Lực</h3>
              <button className="cp2-modal-close" onClick={() => setIsModalVisible(false)} disabled={isSubmitting}>
                &times;
              </button>
            </div>

            <div className="cp2-modal-body custom-scrollbar" style={{ display: 'flex', flexWrap: 'wrap', gap: '1.5rem', maxHeight: '70vh' }}>
              
              {/* Left side: Dynamic Form List */}
              <div style={{ flex: '1 1 500px' }}>
                <form id="config-form" onSubmit={handleSubmit}>
                  {criteriaList.map((item, index) => (
                    <div key={item.key} className="cp2-dynamic-item">
                      <div className="cp2-dynamic-indicator" style={{ backgroundColor: CHART_COLORS[index % CHART_COLORS.length] }}></div>
                      <div className="cp2-dynamic-content">
                        <div style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap' }}>
                          <div className="cp2-form-group" style={{ flex: '1 1 200px' }}>
                            <label className="cp2-form-label">Tên tiêu chí <span className="required">*</span></label>
                            <input
                              type="text"
                              value={item.tenNangLuc}
                              onChange={e => handleCriteriaChange(item.key, 'tenNangLuc', e.target.value)}
                              className="cp2-form-input"
                              placeholder="Kỹ năng giải quyết vấn đề"
                              required
                            />
                          </div>
                          
                          <div className="cp2-form-group" style={{ width: '120px' }}>
                            <label className="cp2-form-label">Tỷ trọng (%) <span className="required">*</span></label>
                            <div style={{ position: 'relative' }}>
                              <input
                                type="number"
                                min="0.1"
                                max="100"
                                step="0.1"
                                value={item.tyTrong}
                                onChange={e => handleCriteriaChange(item.key, 'tyTrong', e.target.value)}
                                className="cp2-form-input"
                                style={{ paddingRight: '1.5rem' }}
                                required
                              />
                              <span style={{ position: 'absolute', right: '0.75rem', top: '50%', transform: 'translateY(-50%)', color: '#9ca3af' }}>%</span>
                            </div>
                          </div>
                        </div>

                        <div className="cp2-form-group" style={{ marginBottom: 0 }}>
                          <label className="cp2-form-label">Mô tả</label>
                          <textarea
                            value={item.moTa}
                            onChange={e => handleCriteriaChange(item.key, 'moTa', e.target.value)}
                            className="cp2-form-textarea"
                            placeholder="Nhập mô tả chi tiết cho tiêu chí này (không bắt buộc)..."
                            style={{ minHeight: '60px' }}
                          />
                        </div>
                      </div>
                      <button 
                        type="button" 
                        className="cp2-btn-remove" 
                        onClick={() => removeCriteria(item.key)}
                        title="Xóa tiêu chí"
                      >
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M15 12H9m12 0a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                      </button>
                    </div>
                  ))}
                  
                  <button type="button" className="cp2-btn cp2-btn-dashed" onClick={addCriteria}>
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1rem', height: '1rem', marginRight: '0.5rem' }}>
                      <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
                    </svg>
                    Thêm tiêu chí mới
                  </button>
                </form>
              </div>

              {/* Right side: Donut Chart & Stats */}
              <div className="cp2-chart-container" style={{ width: '280px', flexShrink: 0 }}>
                <h3 style={{ fontSize: '1rem', fontWeight: 600, color: '#374151', marginBottom: '1.5rem' }}>Phân Bổ Tỷ Trọng</h3>
                
                <div className="cp2-donut-chart" style={{ background: conicGradient }}>
                  <div className="cp2-donut-hole">
                    <span className="cp2-donut-value" style={{ color: isOverweight ? '#ef4444' : '#111827' }}>
                      {totalWeightPercent.toFixed(1)}%
                    </span>
                    <span className="cp2-donut-label">Tổng cộng</span>
                  </div>
                </div>

                <div style={{ marginTop: '2rem', width: '100%' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.5rem', fontSize: '0.875rem' }}>
                    <span style={{ color: '#6b7280' }}>Đã phân bổ:</span>
                    <span style={{ fontWeight: 600, color: isOverweight ? '#ef4444' : '#111827' }}>{totalWeightPercent.toFixed(1)}%</span>
                  </div>
                  <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.875rem' }}>
                    <span style={{ color: '#6b7280' }}>Còn lại:</span>
                    <span style={{ fontWeight: 600, color: '#111827' }}>{Math.max(0, 100 - totalWeightPercent).toFixed(1)}%</span>
                  </div>
                </div>

                {isOverweight && (
                  <div style={{ marginTop: '1rem', padding: '0.75rem', background: '#fef2f2', border: '1px solid #fecaca', color: '#dc2626', fontSize: '0.875rem', borderRadius: '8px', textAlign: 'center' }}>
                    Tổng tỷ trọng đang vượt quá 100%. Vui lòng điều chỉnh lại.
                  </div>
                )}
              </div>

            </div>

            <div className="cp2-modal-footer">
              <button type="button" className="cp2-btn cp2-btn-secondary" onClick={() => setIsModalVisible(false)} disabled={isSubmitting}>
                Hủy bỏ
              </button>
              <button 
                type="submit" 
                form="config-form" 
                className="cp2-btn cp2-btn-primary" 
                disabled={isSubmitting || isOverweight}
              >
                {isSubmitting ? 'Đang lưu...' : 'Lưu lại'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
