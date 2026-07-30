import React, { useState, useEffect, useRef } from 'react';
import { payrollApi } from '../api/payrollApi';
import { workScheduleApi } from '../../workSchedule/api/workScheduleApi';
import { PayrollListDto } from '../types/payroll.types';
import { Toast } from '@/components/Toast/Toast';
import { PayrollDetailModal } from './PayrollDetailModal';
import './PayrollManagement.css';

const PayrollManagement: React.FC = () => {
  const [thang, setThang] = useState<number>(new Date().getMonth() + 1);
  const [nam, setNam] = useState<number>(new Date().getFullYear());
  const [payrolls, setPayrolls] = useState<PayrollListDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [calculating, setCalculating] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);
  const [selectedPayroll, setSelectedPayroll] = useState<PayrollListDto | null>(null);

  const [validYears, setValidYears] = useState<number[]>([]);
  const [isYearDropdownOpen, setIsYearDropdownOpen] = useState(false);
  const [isMonthDropdownOpen, setIsMonthDropdownOpen] = useState(false);

  const yearRef = useRef<HTMLDivElement>(null);
  const monthRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const fetchYears = async () => {
      try {
        const res = await workScheduleApi.getAll();
        if (res.succeeded && res.data) {
          const years = Array.from(new Set(res.data.map((w: any) => w.nam))).sort((a, b) => b - a);
          setValidYears(years);
          if (years.length > 0) {
            const currentYear = new Date().getFullYear();
            const closestYear = years.reduce((prev, curr) => Math.abs(curr - currentYear) < Math.abs(prev - currentYear) ? curr : prev);
            setNam(closestYear);
          }
        }
      } catch (error) {
        console.error('Lỗi khi tải lịch làm việc', error);
      }
    };
    fetchYears();
  }, []);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (yearRef.current && !yearRef.current.contains(e.target as Node)) {
        setIsYearDropdownOpen(false);
      }
      if (monthRef.current && !monthRef.current.contains(e.target as Node)) {
        setIsMonthDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const fetchPayrolls = async () => {
    try {
      setLoading(true);
      const res = await payrollApi.getPayrollList(thang, nam);
      if (res.succeeded) {
        setPayrolls(res.data || []);
      }
    } catch (error) {
      console.error('Lỗi khi tải bảng lương', error);
      setToast({ message: 'Không thể tải bảng lương. Vui lòng kiểm tra lại!', type: 'error' });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPayrolls();
  }, [thang, nam]);

  const handleCalculate = async () => {
    if (!window.confirm(`Bạn có chắc chắn muốn tính lương cho tháng ${thang}/${nam} không? Dữ liệu cũ của kỳ này (nếu có và chưa chốt) sẽ bị thay thế.`)) return;
    
    try {
      setCalculating(true);
      const res = await payrollApi.calculatePayroll({ thang, nam });
      if (res.succeeded) {
        setToast({ message: 'Tính lương thành công!', type: 'success' });
        fetchPayrolls();
      }
    } catch (error: any) {
      console.error('Lỗi khi tính lương', error);
      const msg = error.response?.data?.message || 'Có lỗi xảy ra khi tính lương.';
      setToast({ message: msg, type: 'error' });
    } finally {
      setCalculating(false);
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
  };

  return (
    <div className="payroll-management">
      <div className="payroll-header">
        <h2>💰 Bảng tính lương</h2>
        <div className="payroll-actions">
          {validYears.length > 0 && (
            <>
              <div className="filter-group">
                <label>Tháng:</label>
                <div className="custom-dropdown-wrap" ref={monthRef}>
                  <div className="custom-dropdown-input" onClick={() => setIsMonthDropdownOpen(!isMonthDropdownOpen)}>
                    <span>Tháng {thang}</span>
                    <span className="dropdown-arrow"></span>
                  </div>
                  {isMonthDropdownOpen && (
                    <ul className="custom-dropdown-list">
                      {Array.from({ length: 12 }, (_, i) => i + 1).map(m => (
                        <li key={m} className={thang === m ? 'selected' : ''} onClick={() => { setThang(m); setIsMonthDropdownOpen(false); }}>
                          Tháng {m}
                        </li>
                      ))}
                    </ul>
                  )}
                </div>

                <label>Năm:</label>
                <div className="custom-dropdown-wrap" ref={yearRef}>
                  <div className="custom-dropdown-input" onClick={() => setIsYearDropdownOpen(!isYearDropdownOpen)}>
                    <span>Năm {nam}</span>
                    <span className="dropdown-arrow"></span>
                  </div>
                  {isYearDropdownOpen && (
                    <ul className="custom-dropdown-list">
                      {validYears.map(y => (
                        <li key={y} className={nam === y ? 'selected' : ''} onClick={() => { setNam(y); setIsYearDropdownOpen(false); }}>
                          Năm {y}
                        </li>
                      ))}
                    </ul>
                  )}
                </div>
              </div>
              <button 
                className="btn-primary" 
                onClick={handleCalculate}
                disabled={calculating}
              >
                {calculating ? 'Đang tính...' : 'Chạy tính lương'}
              </button>
            </>
          )}
        </div>
      </div>

      <div className="payroll-table-container">
        {loading ? (
          <div className="loading-state">Đang tải dữ liệu...</div>
        ) : payrolls.length === 0 ? (
          <div className="empty-state">Chưa có dữ liệu lương của tháng {thang}/{nam}. Hãy nhấn "Chạy tính lương" để bắt đầu.</div>
        ) : (
          <table className="payroll-table">
            <thead>
              <tr>
                <th>Nhân viên</th>
                <th>Phòng ban</th>
                <th>Lương P1</th>
                <th>Công (TT/Ch)</th>
                <th>Lương thời gian</th>
                <th>Lương hiệu suất</th>
                <th>Khấu trừ</th>
                <th>Thực lĩnh</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {payrolls.map((row) => (
                <tr key={row.idBangLuong}>
                  <td>
                    <div className="emp-name">{row.tenNhanVien}</div>
                    <div className="emp-id">{row.cccdNhanVien}</div>
                  </td>
                  <td>
                    <div className="dept-name">{row.tenPhongBan}</div>
                    <div className="position-name">{row.tenChucVu}</div>
                  </td>
                  <td className="text-right fw-bold text-primary">{formatCurrency(row.p1)}</td>
                  <td className="text-center">
                    <div>{row.ngayCongThucTe} / {row.ngayCongChuan} ngày</div>
                    <div style={{ fontSize: '0.85em', color: 'var(--text-secondary)' }}>
                      {row.gioCongThucTe} / {row.gioCongChuan} giờ
                    </div>
                  </td>
                  <td className="text-right">{formatCurrency(row.luongThoiGian)}</td>
                  <td className="text-right">{formatCurrency(row.luongHieuSuatP3)}</td>
                  <td className="text-right text-danger">0 đ</td>
                  <td className="text-right fw-bold text-success">{formatCurrency(row.thucLinh)}</td>
                  <td className="text-center">
                    <button 
                      className="btn-outline" 
                      onClick={() => setSelectedPayroll(row)}
                      title="Xem chi tiết"
                    >
                      Chi tiết
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}

      {selectedPayroll && (
        <PayrollDetailModal 
          payroll={selectedPayroll} 
          onClose={() => setSelectedPayroll(null)} 
        />
      )}
    </div>
  );
};

export default PayrollManagement;
