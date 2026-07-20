import React, { useEffect, useState } from 'react';
import { useWorkSchedule } from '../hooks/useWorkSchedule';
import { WorkScheduleDetailModal } from './WorkScheduleDetailModal';
import { useAuthStore } from '@/store/useAuthStore';
import type { LichLamViecDto } from '../types/workSchedule.types';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { workShiftApi } from '../../workShifts/api/workShiftApi';
import type { CaLamViec } from '../../workShifts/types';
import './WorkScheduleManagement.css';

const currentYear = new Date().getFullYear();
const YEAR_OPTIONS = Array.from({ length: 11 }, (_, i) => currentYear - 2 + i);

export const WorkScheduleManagement: React.FC = () => {
  const { lichList, isLoading, isCreating, error, successMsg, fetchAll, create, remove, clearMessages } =
    useWorkSchedule();

  const { user } = useAuthStore();
  const canManage = user?.role === 'Admin' || user?.role === 'HR';

  const [selectedYear, setSelectedYear] = useState(currentYear);
  const [createNotes, setCreateNotes] = useState('');
  const [useDefaultShift, setUseDefaultShift] = useState(false);
  const [defaultShiftId, setDefaultShiftId] = useState<string>('');
  const [shifts, setShifts] = useState<CaLamViec[]>([]);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);
  const [viewLich, setViewLich] = useState<LichLamViecDto | null>(null);
  const [confirmDelete, setConfirmDelete] = useState<LichLamViecDto | null>(null);

  const fetchShifts = async () => {
    try {
      const res = await workShiftApi.getAll();
      if (res.succeeded) {
        setShifts(res.data);
      }
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    fetchShifts();
  }, []);

  useEffect(() => {
    fetchAll();
  }, [fetchAll]);

  useEffect(() => {
    if (error || successMsg) {
      const t = setTimeout(clearMessages, 4000);
      return () => clearTimeout(t);
    }
  }, [error, successMsg, clearMessages]);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (!(e.target as Element).closest('.ws-td-actions')) {
        setOpenDropdownId(null);
      }
    };
    document.addEventListener('click', handleClickOutside);
    return () => document.removeEventListener('click', handleClickOutside);
  }, []);

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
    data: lichList,
    initialPageSize: 10,
    searchableFields: ['nam', 'ghiChu', 'trangThai']
  });

  const handleExportExcel = () => {
    const columns: ExportColumn<any>[] = [
      { header: 'Năm', key: 'nam' },
      { header: 'Ngày làm việc', key: 'tongNgayLam' },
      { header: 'Nghỉ T7 & CN', key: 'tongNgayNghiCuoiTuan' },
      { header: 'Nghỉ lễ', key: 'tongNgayLe' },
      { header: 'Tổng ngày', key: 'tongNgay' },
      { header: 'Trạng thái', key: 'trangThai' },
      { header: 'Ghi chú', key: 'ghiChu' },
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'LichLamViec');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<any>[] = [
      { header: 'Năm', key: 'nam' },
      { header: 'Ngày làm việc', key: 'tongNgayLam' },
      { header: 'Nghỉ T7 & CN', key: 'tongNgayNghiCuoiTuan' },
      { header: 'Nghỉ lễ', key: 'tongNgayLe' },
      { header: 'Tổng ngày', key: 'tongNgay' },
      { header: 'Trạng thái', key: 'trangThai' },
      { header: 'Ghi chú', key: 'ghiChu' },
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'LichLamViec', 'Lịch làm việc');
  };

  const handleCreate = async () => {
    const yearExists = lichList.some((l) => l.nam === selectedYear);
    if (yearExists) {
      return; 
    }
    const success = await create({ 
      nam: selectedYear, 
      ghiChu: createNotes,
      useDefaultShift,
      defaultShiftId: useDefaultShift ? defaultShiftId : undefined
    });
    if (success) {
      setShowCreateModal(false);
      setCreateNotes('');
      setUseDefaultShift(false);
      setDefaultShiftId('');
    }
  };

  const handleConfirmDelete = async () => {
    if (!confirmDelete) return;
    await remove(confirmDelete.idLich, confirmDelete.nam);
    setConfirmDelete(null);
  };

  const existingYears = new Set(lichList.map((l) => l.nam));
  const isYearExists = existingYears.has(selectedYear);

  return (
    <div className="ws-container">
      {/* Header */}
      <div className="ws-header">
        <div className="ws-header-left">
          <h1>📋 Lịch làm việc</h1>
          <p>Tạo và quản lý lịch làm việc theo năm với ngày lễ Việt Nam</p>
        </div>
        <div className="ws-header-actions">
          {canManage && (
            <button
              id="ws-btn-create"
              className="ws-btn-create"
              onClick={() => {
                setSelectedYear(currentYear);
                setCreateNotes('');
                setUseDefaultShift(false);
                setDefaultShiftId('');
                setShowCreateModal(true);
              }}
              title="Tạo lịch làm việc mới"
            >
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
              </svg>
              Tạo lịch làm việc
            </button>
          )}
        </div>
      </div>
      
      <div style={{ display: 'flex', justifyContent: 'space-between', padding: '1rem', borderBottom: '1px solid var(--border-color)', gap: '1rem', flexWrap: 'wrap' }}>
        <div className="ws-input-wrapper" style={{ flex: 1, minWidth: '250px', position: 'relative' }}>
          <input
            type="text"
            placeholder="Tìm kiếm lịch..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="ws-input"
            style={{ width: '100%', padding: '0.5rem 1rem', borderRadius: '4px', border: '1px solid var(--border-color)' }}
          />
        </div>
        <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
      </div>

      {/* Alert messages */}
      {successMsg && (
        <div className="ws-alert success" role="alert">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem', flexShrink: 0 }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M9 12.75 11.25 15 15 9.75M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
          </svg>
          {successMsg}
        </div>
      )}
      {error && (
        <div className="ws-alert error" role="alert">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem', flexShrink: 0 }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m9-.75a9 9 0 1 1-18 0 9 9 0 0 1 18 0Zm-9 3.75h.008v.008H12v-.008Z" />
          </svg>
          {error}
        </div>
      )}

      {/* Table */}
      <div className="ws-table-wrapper">
        {isLoading && lichList.length === 0 ? (
          <div className="ws-spinner" />
        ) : lichList.length === 0 ? (
          <div className="ws-empty">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 0 1 2.25-2.25h13.5A2.25 2.25 0 0 1 21 7.5v11.25m-18 0A2.25 2.25 0 0 0 5.25 21h13.5A2.25 2.25 0 0 0 21 18.75m-18 0v-7.5A2.25 2.25 0 0 1 5.25 9h13.5A2.25 2.25 0 0 1 21 11.25v7.5" />
            </svg>
            <p>Chưa có lịch làm việc nào. Hãy tạo lịch cho năm đầu tiên!</p>
          </div>
        ) : (
          <table className="ws-table" aria-label="Danh sách lịch làm việc">
            <thead>
              <tr>
                <SortableHeader label="Năm" sortKey="nam" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Ngày làm việc" sortKey="tongNgayLam" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Nghỉ T7 & CN" sortKey="tongNgayNghiCuoiTuan" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Nghỉ lễ" sortKey="tongNgayLe" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Tổng ngày" sortKey="tongNgay" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Trạng thái" sortKey="trangThai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <th>Ghi chú</th>
                <th style={{ textAlign: 'center' }}>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {currentData.map((lich) => (
                <tr key={lich.idLich}>
                  <td className="ws-td-year">{lich.nam}</td>
                  <td>
                    <span className="ws-stat-chip work">
                      <span style={{ width: 6, height: 6, borderRadius: '50%', background: 'var(--success-text)', display: 'inline-block' }} />
                      {lich.tongNgayLam}
                    </span>
                  </td>
                  <td>
                    <span className="ws-stat-chip weekend">
                      <span style={{ width: 6, height: 6, borderRadius: '50%', background: 'var(--primary)', display: 'inline-block' }} />
                      {lich.tongNgayNghiCuoiTuan}
                    </span>
                  </td>
                  <td>
                    <span className="ws-stat-chip holiday">
                      <span style={{ width: 6, height: 6, borderRadius: '50%', background: 'var(--warning-text)', display: 'inline-block' }} />
                      {lich.tongNgayLe}
                    </span>
                  </td>
                  <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{lich.tongNgay}</td>
                  <td>
                    <span className={`ws-badge ${lich.trangThai === 'Hiệu lực' ? 'active' : 'inactive'}`}>
                      <span style={{ width: 6, height: 6, borderRadius: '50%', background: lich.trangThai === 'Hiệu lực' ? 'var(--success-text)' : 'var(--text-muted)', display: 'inline-block' }} />
                      {lich.trangThai}
                    </span>
                  </td>
                  <td className="ws-td-note">{lich.ghiChu || '-'}</td>
                  <td className="ws-td-actions" style={{ position: 'relative' }}>
                    <div style={{ display: 'flex', justifyContent: 'center' }}>
                      <button
                        className="ws-btn-actions"
                        onClick={(e) => {
                          e.stopPropagation();
                          setOpenDropdownId(openDropdownId === lich.idLich ? null : lich.idLich);
                        }}
                        aria-label="Thao tác"
                        title="Thao tác"
                      >
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 12.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 18.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5Z" />
                        </svg>
                      </button>

                      {openDropdownId === lich.idLich && (
                        <div className="ws-actions-dropdown">
                          <button
                            className="ws-dropdown-item"
                            onClick={() => {
                              setViewLich(lich);
                              setOpenDropdownId(null);
                            }}
                          >
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '0.9rem', height: '0.9rem' }}>
                              <path strokeLinecap="round" strokeLinejoin="round" d="M2.036 12.322a1.012 1.012 0 0 1 0-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178Z" />
                              <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
                            </svg>
                            Xem chi tiết
                          </button>
                          {canManage && (
                            <button
                              className="ws-dropdown-item delete"
                              onClick={() => {
                                setConfirmDelete(lich);
                                setOpenDropdownId(null);
                              }}
                            >
                              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '0.9rem', height: '0.9rem' }}>
                                <path strokeLinecap="round" strokeLinejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
                              </svg>
                              Xóa lịch
                            </button>
                          )}
                        </div>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
        
        {totalPages > 0 && (
          <div className="ws-pagination" style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.5rem', marginTop: '1rem', padding: '0 1rem 1rem 1rem' }}>
            <button 
              className="ws-btn-secondary" 
              onClick={() => setCurrentPage(p => p - 1)} 
              disabled={currentPage === 1 || isLoading}
              style={{ padding: '0.35rem 0.75rem', border: '1px solid var(--border-color)', borderRadius: '4px', background: 'var(--bg-surface)' }}
            >
              Trước
            </button>
            <div className="ws-pagination-info" style={{ display: 'flex', alignItems: 'center' }}>
              Trang <span>{currentPage}</span> / <span>{totalPages}</span>
            </div>
            <button 
              className="ws-btn-secondary" 
              onClick={() => setCurrentPage(p => p + 1)} 
              disabled={currentPage === totalPages || isLoading}
              style={{ padding: '0.35rem 0.75rem', border: '1px solid var(--border-color)', borderRadius: '4px', background: 'var(--bg-surface)' }}
            >
              Sau
            </button>
          </div>
        )}
      </div>

      {/* Detail Modal */}
      {viewLich && (
        <WorkScheduleDetailModal 
          lich={viewLich} 
          onClose={(hasChanges?: boolean) => {
            setViewLich(null);
            if (hasChanges) {
              fetchAll();
            }
          }} 
        />
      )}

      {/* Confirm Delete Dialog */}
      {confirmDelete && (
        <div className="ws-confirm-overlay">
          <div className="ws-confirm-box">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="var(--danger-text)" style={{ width: '2.5rem', height: '2.5rem', marginBottom: '0.75rem' }}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z" />
            </svg>
            <h3>Xác nhận xóa lịch</h3>
            <p>
              Bạn có chắc muốn xóa lịch làm việc năm <strong>{confirmDelete.nam}</strong>?
              <br />Hành động này không thể hoàn tác.
            </p>
            <div className="ws-confirm-actions">
              <button
                id="ws-confirm-cancel"
                className="ws-confirm-cancel"
                onClick={() => setConfirmDelete(null)}
              >
                Hủy bỏ
              </button>
              <button
                id="ws-confirm-ok"
                className="ws-confirm-ok"
                onClick={handleConfirmDelete}
              >
                Xác nhận xóa
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Create Schedule Modal */}
      {showCreateModal && (
        <div className="ws-modal-overlay">
          <div className="ws-modal" style={{ maxWidth: '500px', width: '90%', height: 'auto', display: 'flex', flexDirection: 'column' }}>
            <div className="ws-modal-header" style={{ padding: '1.25rem 1.5rem' }}>
              <h2 className="ws-modal-title" style={{ fontSize: '1.25rem', margin: 0 }}>Tạo lịch làm việc</h2>
              <button 
                className="ws-modal-close"
                onClick={() => setShowCreateModal(false)}
                title="Đóng"
                disabled={isCreating}
              >
                &times;
              </button>
            </div>
            
            <div className="ws-modal-body" style={{ padding: '1.5rem', flex: 'none' }}>
              <div style={{ marginBottom: '1.25rem' }}>
                <label htmlFor="ws-modal-year" style={{ display: 'block', marginBottom: '0.5rem', fontSize: '0.9rem', fontWeight: 600, color: 'var(--text-primary)' }}>Chọn năm <span style={{color: 'var(--danger-text)'}}>*</span></label>
                <select
                  id="ws-modal-year"
                  className="ws-year-select"
                  style={{ width: '100%' }}
                  value={selectedYear}
                  onChange={(e) => setSelectedYear(Number(e.target.value))}
                >
                  {YEAR_OPTIONS.map((y) => (
                    <option key={y} value={y}>{y}</option>
                  ))}
                </select>
                
                {isYearExists && (
                  <div style={{ marginTop: '0.5rem', color: 'var(--danger-text)', fontSize: '0.875rem', display: 'flex', alignItems: 'center', gap: '0.35rem' }}>
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                      <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z" />
                    </svg>
                    Lịch làm việc năm {selectedYear} đã tồn tại!
                  </div>
                )}
              </div>

              <div style={{ marginBottom: '1.5rem' }}>
                <label htmlFor="ws-modal-notes" style={{ display: 'block', marginBottom: '0.5rem', fontSize: '0.9rem', fontWeight: 600, color: 'var(--text-primary)' }}>Ghi chú</label>
                <textarea
                  id="ws-modal-notes"
                  value={createNotes}
                  onChange={(e) => setCreateNotes(e.target.value)}
                  placeholder="Nhập ghi chú (không bắt buộc)..."
                  style={{ width: '100%', padding: '0.75rem', border: '1px solid var(--border-color)', borderRadius: '8px', minHeight: '100px', fontFamily: 'inherit', resize: 'vertical' }}
                />
              </div>

              <div style={{ marginBottom: '1.5rem', display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
                <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', fontSize: '0.9rem', fontWeight: 600, color: 'var(--text-primary)', cursor: 'pointer' }}>
                  <input
                    type="checkbox"
                    checked={useDefaultShift}
                    onChange={(e) => setUseDefaultShift(e.target.checked)}
                    style={{ width: '1.1rem', height: '1.1rem', cursor: 'pointer' }}
                  />
                  Gán ca làm việc mặc định cho ngày làm việc
                </label>

                {useDefaultShift && (
                  <div>
                    <select
                      className="ws-year-select"
                      style={{ width: '100%' }}
                      value={defaultShiftId}
                      onChange={(e) => setDefaultShiftId(e.target.value)}
                    >
                      <option value="" disabled>-- Chọn ca làm việc --</option>
                      {shifts.map(shift => (
                        <option key={shift.id} value={shift.id}>
                          {shift.tenCa} ({shift.gioBatDau} - {shift.gioKetThuc})
                        </option>
                      ))}
                    </select>
                    {!defaultShiftId && (
                       <div style={{ marginTop: '0.5rem', color: 'var(--danger-text)', fontSize: '0.875rem' }}>
                         Vui lòng chọn ca làm việc.
                       </div>
                    )}
                  </div>
                )}
              </div>
              
              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '1rem', marginTop: '1rem' }}>
                <button
                  onClick={() => setShowCreateModal(false)}
                  disabled={isCreating}
                  style={{ padding: '0.625rem 1.25rem', border: '1px solid var(--border-hover)', background: 'var(--bg-surface)', borderRadius: '8px', fontWeight: 600, color: 'var(--text-secondary)', cursor: 'pointer' }}
                >
                  Hủy bỏ
                </button>
                <button
                  className="ws-btn-create"
                  onClick={handleCreate}
                  disabled={isCreating || isYearExists || (useDefaultShift && !defaultShiftId)}
                  style={{ padding: '0.625rem 1.5rem', borderRadius: '8px' }}
                >
                  {isCreating ? (
                    <>
                      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1rem', height: '1rem', animation: 'spin 0.7s linear infinite' }}>
                        <path strokeLinecap="round" strokeLinejoin="round" d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0 3.181 3.183a8.25 8.25 0 0 0 13.803-3.7M4.031 9.865a8.25 8.25 0 0 1 13.803-3.7l3.181 3.182m0-4.991v4.99" />
                      </svg>
                      Đang tạo...
                    </>
                  ) : 'Tạo lịch'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
