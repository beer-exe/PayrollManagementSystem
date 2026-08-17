import React, { useState, useEffect, useMemo } from 'react';
import './MySchedule.css';
import { useMySchedule } from '../hooks/useMySchedule';
import { useWorkSchedule } from '../../workSchedule/hooks/useWorkSchedule';
import { Toast } from '@/components/Toast/Toast';

const DAYS_OF_WEEK = ['Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7', 'Chủ nhật'];

const MySchedule: React.FC = () => {
  const [currentDate, setCurrentDate] = useState(new Date());
  const { schedule, isLoading, fetchSchedule, toast, setToast } = useMySchedule();
  const { lichList, fetchAll: fetchLichList } = useWorkSchedule();

  useEffect(() => {
    fetchLichList();
  }, [fetchLichList]);

  const availableYears = useMemo(() => {
    return Array.from(new Set(lichList.map(l => l.nam))).sort((a, b) => b - a);
  }, [lichList]);

  useEffect(() => {
    // If the currently selected year is not in the available years, default to the latest available year
    if (availableYears.length > 0 && !availableYears.includes(currentDate.getFullYear())) {
      setCurrentDate(prev => new Date(availableYears[0], prev.getMonth(), 1));
    }
  }, [availableYears]);

  useEffect(() => {
    fetchSchedule(currentDate.getMonth() + 1, currentDate.getFullYear());
  }, [currentDate, fetchSchedule]);

  const handlePrevMonth = () => {
    setCurrentDate(prev => new Date(prev.getFullYear(), prev.getMonth() - 1, 1));
  };

  const handleNextMonth = () => {
    setCurrentDate(prev => new Date(prev.getFullYear(), prev.getMonth() + 1, 1));
  };

  // Calendar calculations
  const calendarCells = useMemo(() => {
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);

    // JS getDay(): 0 is Sunday, 1 is Monday. We want 0 is Monday, 6 is Sunday.
    let startDayIndex = firstDay.getDay() - 1;
    if (startDayIndex === -1) startDayIndex = 6;

    const cells = [];

    // Empty cells before start of month
    const prevMonthLastDay = new Date(year, month, 0).getDate();
    for (let i = startDayIndex - 1; i >= 0; i--) {
      cells.push({ type: 'empty', key: `empty-${i}`, date: prevMonthLastDay - i });
    }

    // Days of the month
    const today = new Date();
    for (let i = 1; i <= lastDay.getDate(); i++) {
      const dateStr = `${year}-${String(month + 1).padStart(2, '0')}-${String(i).padStart(2, '0')}`;
      const dayData = schedule.find(s => s.ngay.startsWith(dateStr));
      const isToday = i === today.getDate() && month === today.getMonth() && year === today.getFullYear();
      cells.push({ type: 'day', key: `day-${i}`, date: i, data: dayData, isToday });
    }

    // Empty cells after end of month (to complete the grid)
    const remainingCells = (7 - (cells.length % 7)) % 7;
    for (let i = 1; i <= remainingCells; i++) {
      cells.push({ type: 'empty', key: `empty-end-${i}`, date: i });
    }

    return cells;
  }, [currentDate, schedule]);

  const renderBadge = (dayData: any) => {
    if (!dayData) return null;

    if (dayData.coNghiPhep) {
      return <span className="psch-badge psch-badge-leave">{dayData.loaiNghiPhep} (Đã duyệt)</span>;
    }

    if (dayData.laCaDuocPhanCong) {
      return <span className="psch-badge psch-badge-assigned">Ca phân công: {dayData.tenCa}</span>;
    }

    if (dayData.loaiNgay === 'Ngày làm việc') {
      return <span className="psch-badge psch-badge-work">Ca mặc định: {dayData.tenCa || 'Không có ca'}</span>;
    }

    return <span className="psch-badge psch-badge-holiday">{dayData.tenNgayNghi || dayData.loaiNgay}</span>;
  };

  return (
    <div className="psch-container">
      <div className="psch-header">
        <div className="psch-title">
          <h2>Lịch làm việc cá nhân</h2>
        </div>
        <div className="psch-controls">
          <button
            className="psch-btn-primary"
            onClick={handlePrevMonth}
            disabled={currentDate.getMonth() === 0 || availableYears.length === 0}
          >
            &lt; Tháng trước
          </button>
          <select
            className="psch-select"
            value={currentDate.getMonth()}
            onChange={(e) => setCurrentDate(new Date(currentDate.getFullYear(), parseInt(e.target.value), 1))}
          >
            {Array.from({ length: 12 }, (_, i) => (
              <option key={i} value={i}>Tháng {i + 1}</option>
            ))}
          </select>
          <select
            className="psch-select"
            value={currentDate.getFullYear()}
            onChange={(e) => setCurrentDate(new Date(parseInt(e.target.value), currentDate.getMonth(), 1))}
            disabled={availableYears.length === 0}
          >
            {availableYears.length > 0 ? (
              availableYears.map(y => (
                <option key={y} value={y}>Năm {y}</option>
              ))
            ) : (
              <option value={currentDate.getFullYear()}>-- Chưa có lịch làm việc --</option>
            )}
          </select>
          <button
            className="psch-btn-primary"
            onClick={handleNextMonth}
            disabled={currentDate.getMonth() === 11 || availableYears.length === 0}
          >
            Tháng sau &gt;
          </button>
        </div>
      </div>

      {availableYears.length === 0 && !isLoading ? (
        <div style={{ padding: '64px 24px', textAlign: 'center', backgroundColor: 'var(--bg-main)', border: '1px dashed var(--border-color)', borderRadius: '8px', margin: '24px 0' }}>
          <div style={{ marginBottom: '16px', display: 'flex', justifyContent: 'center' }}>
            <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="var(--text-muted)" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round">
              <rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect>
              <line x1="16" y1="2" x2="16" y2="6"></line>
              <line x1="8" y1="2" x2="8" y2="6"></line>
              <line x1="3" y1="10" x2="21" y2="10"></line>
            </svg>
          </div>
          <h3 style={{ marginBottom: '8px', fontSize: '1.1rem', fontWeight: 500, color: 'var(--text-primary)' }}>Chưa có dữ liệu Lịch làm việc</h3>
        </div>
      ) : (
        <>
          <div className="psch-calendar">
            <div className="psch-calendar-header">
              {DAYS_OF_WEEK.map(day => (
                <div key={day} className="psch-calendar-header-cell">{day}</div>
              ))}
            </div>

            {isLoading ? (
              <div className="psch-loading">Đang tải dữ liệu...</div>
            ) : (
              <div className="psch-calendar-grid">
                {calendarCells.map((cell: any) => {
                  if (cell.type === 'empty') {
                    return (
                      <div key={cell.key} className="psch-day-cell psch-empty">
                        {cell.date && (
                          <div className="psch-date-number" style={{ color: 'var(--text-muted)' }}>
                            {cell.date}
                          </div>
                        )}
                      </div>
                    );
                  }

                  const { data } = cell;

                  return (
                    <div key={cell.key} className={`psch-day-cell ${cell.isToday ? 'psch-today-cell' : ''}`}>
                      <div className={`psch-date-number ${cell.isToday ? 'psch-today' : ''}`}>{cell.date}</div>
                      <div className="psch-shift-info">
                        {renderBadge(data)}
                        {data && (data.laCaDuocPhanCong || data.loaiNgay === 'Ngày làm việc') && !data.coNghiPhep && data.gioBatDau && data.gioKetThuc && (
                          <div className="psch-time">
                            🕒 {data.gioBatDau.substring(0, 5)} - {data.gioKetThuc.substring(0, 5)}
                            {data.xuyenNgay && " (+1 ngày)"}
                          </div>
                        )}
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>

          <div className="psch-legend">
            <div className="psch-legend-item">
              <div className="psch-legend-color" style={{ backgroundColor: 'var(--primary-light)', border: '1px solid var(--primary-border)' }}></div>
              <span>Ca làm việc mặc định</span>
            </div>
            <div className="psch-legend-item">
              <div className="psch-legend-color" style={{ backgroundColor: 'var(--success-bg)', border: '1px solid var(--success)' }}></div>
              <span>Ca phân công riêng</span>
            </div>
            <div className="psch-legend-item">
              <div className="psch-legend-color" style={{ backgroundColor: 'var(--danger-bg)', border: '1px solid var(--danger)' }}></div>
              <span>Ngày nghỉ / Lễ</span>
            </div>
            <div className="psch-legend-item">
              <div className="psch-legend-color" style={{ backgroundColor: 'var(--warning-bg)', border: '1px solid var(--warning)' }}></div>
              <span>Nghỉ phép (Đã duyệt)</span>
            </div>
          </div>
        </>
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

export default MySchedule;
