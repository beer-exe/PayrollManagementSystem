import React, { useEffect, useState } from 'react';
import { useWorkSchedule } from '../hooks/useWorkSchedule';
import { WorkScheduleDetailModal } from './WorkScheduleDetailModal';
import { useAuthStore } from '@/store/useAuthStore';
import type { LichLamViecDto } from '../types/workSchedule.types';
import './WorkScheduleManagement.css';

const currentYear = new Date().getFullYear();
const YEAR_OPTIONS = Array.from({ length: 11 }, (_, i) => currentYear - 2 + i);

export const WorkScheduleManagement: React.FC = () => {
  const { lichList, isLoading, isCreating, error, successMsg, fetchAll, create, remove, clearMessages } =
    useWorkSchedule();
  
  const { user } = useAuthStore();
  const canManage = user?.role === 'Admin' || user?.role === 'HR';

  const [selectedYear, setSelectedYear] = useState(currentYear);
  const [viewLich, setViewLich] = useState<LichLamViecDto | null>(null);
  const [confirmDelete, setConfirmDelete] = useState<LichLamViecDto | null>(null);

  useEffect(() => {
    fetchAll();
  }, [fetchAll]);

  useEffect(() => {
    if (error || successMsg) {
      const t = setTimeout(clearMessages, 4000);
      return () => clearTimeout(t);
    }
  }, [error, successMsg, clearMessages]);

  const handleCreate = async () => {
    const yearExists = lichList.some((l) => l.nam === selectedYear);
    if (yearExists) {
      return; // Backend sẽ trả lỗi, nhưng UX tốt hơn là check trước
    }
    await create({ nam: selectedYear });
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
          <select
            id="ws-year-select"
            className="ws-year-select"
            value={selectedYear}
            onChange={(e) => setSelectedYear(Number(e.target.value))}
            aria-label="Chọn năm"
          >
            {YEAR_OPTIONS.map((y) => (
              <option key={y} value={y}>{y}</option>
            ))}
          </select>

          {canManage && (
            <button
              id="ws-btn-create"
              className="ws-btn-create"
              onClick={handleCreate}
              disabled={isCreating || isYearExists}
              title={isYearExists ? `Lịch năm ${selectedYear} đã tồn tại` : `Tạo lịch làm việc năm ${selectedYear}`}
            >
              {isCreating ? (
                <>
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1rem', height: '1rem', animation: 'spin 0.7s linear infinite' }}>
                    <path strokeLinecap="round" strokeLinejoin="round" d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0 3.181 3.183a8.25 8.25 0 0 0 13.803-3.7M4.031 9.865a8.25 8.25 0 0 1 13.803-3.7l3.181 3.182m0-4.991v4.99" />
                  </svg>
                  Đang tạo...
                </>
              ) : (
                <>
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
                  </svg>
                  Tạo lịch năm {selectedYear}
                </>
              )}
            </button>
          )}
        </div>
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

      {/* Year exists warning */}
      {isYearExists && (
        <div className="ws-alert" style={{ background: '#fff7ed', border: '1px solid #fed7aa', color: '#92400e' }} role="status">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem', flexShrink: 0 }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z" />
          </svg>
          Lịch làm việc năm <strong>{selectedYear}</strong> đã tồn tại. Vui lòng chọn năm khác.
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
                <th>Năm</th>
                <th>Ngày làm việc</th>
                <th>Nghỉ T7 &amp; CN</th>
                <th>Nghỉ lễ</th>
                <th>Tổng ngày</th>
                <th>Trạng thái</th>
                <th>Ghi chú</th>
                <th style={{ textAlign: 'center' }}>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {lichList.map((lich) => (
                <tr key={lich.idLich}>
                  <td className="ws-td-year">{lich.nam}</td>
                  <td>
                    <span className="ws-stat-chip work">
                      <span style={{ width: 6, height: 6, borderRadius: '50%', background: '#22c55e', display: 'inline-block' }} />
                      {lich.tongNgayLam}
                    </span>
                  </td>
                  <td>
                    <span className="ws-stat-chip weekend">
                      <span style={{ width: 6, height: 6, borderRadius: '50%', background: '#3b82f6', display: 'inline-block' }} />
                      {lich.tongNgayNghiCuoiTuan}
                    </span>
                  </td>
                  <td>
                    <span className="ws-stat-chip holiday">
                      <span style={{ width: 6, height: 6, borderRadius: '50%', background: '#f97316', display: 'inline-block' }} />
                      {lich.tongNgayLe}
                    </span>
                  </td>
                  <td style={{ fontWeight: 600, color: '#374151' }}>{lich.tongNgay}</td>
                  <td>
                    <span className={`ws-badge ${lich.trangThai === 'Hiệu lực' ? 'active' : 'inactive'}`}>
                      <span style={{ width: 6, height: 6, borderRadius: '50%', background: lich.trangThai === 'Hiệu lực' ? '#22c55e' : '#94a3b8', display: 'inline-block' }} />
                      {lich.trangThai}
                    </span>
                  </td>
                  <td style={{ fontSize: '0.82rem', color: '#6b7280', maxWidth: '150px' }}>
                    {lich.ghiChu ?? '—'}
                  </td>
                  <td>
                    <div style={{ display: 'flex', gap: '0.5rem', justifyContent: 'center' }}>
                      <button
                        id={`ws-view-${lich.idLich}`}
                        className="ws-btn-view"
                        onClick={() => setViewLich(lich)}
                        aria-label={`Xem chi tiết lịch năm ${lich.nam}`}
                      >
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '0.9rem', height: '0.9rem' }}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M2.036 12.322a1.012 1.012 0 0 1 0-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178Z" />
                          <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
                        </svg>
                        Xem chi tiết
                      </button>
                      {canManage && (
                        <button
                          id={`ws-delete-${lich.idLich}`}
                          className="ws-btn-delete"
                          onClick={() => setConfirmDelete(lich)}
                          aria-label={`Xóa lịch năm ${lich.nam}`}
                        >
                          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '0.9rem', height: '0.9rem' }}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
                          </svg>
                          Xóa
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Detail Modal */}
      {viewLich && (
        <WorkScheduleDetailModal lich={viewLich} onClose={() => setViewLich(null)} />
      )}

      {/* Confirm Delete Dialog */}
      {confirmDelete && (
        <div className="ws-confirm-overlay">
          <div className="ws-confirm-box">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="#dc2626" style={{ width: '2.5rem', height: '2.5rem', marginBottom: '0.75rem' }}>
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
    </div>
  );
};
