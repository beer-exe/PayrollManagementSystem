import React, { useState } from 'react';
import { useMyPayroll } from '../hooks/useMyPayroll';
import { MyPayrollDto } from '../types/myPayroll.types';
import { PayslipModal } from './PayslipModal';
import { Toast } from '@/components/Toast/Toast';
import './MyPayroll.css';

export const MyPayroll: React.FC = () => {
  const currentYear = new Date().getFullYear();
  const { data, loading, error, year, setYear, toast, setToast, refetch } = useMyPayroll(currentYear);
  const [selectedPayslip, setSelectedPayslip] = useState<MyPayrollDto | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  
  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  // Generate year options (last 5 years)
  const yearOptions = Array.from({ length: 5 }, (_, i) => currentYear - i);

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
  };

  const getStatusClass = (status: string) => {
    switch (status) {
      case 'Đã thanh toán': return 'success';
      case 'Đã chốt': return 'warning';
      default: return 'neutral';
    }
  };

  // Filter and paginate data
  const filteredData = data.filter(item => 
    item.thang.toString().includes(searchTerm)
  );

  const totalPages = Math.ceil(filteredData.length / itemsPerPage);
  const currentData = filteredData.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );

  return (
    <div className="mpay-container">
      <div className="mpay-header">
        <div className="mpay-header-title">
          <h2>💰 Bảng lương của tôi</h2>
          <p>Xem lịch sử chi trả và chi tiết phiếu lương hàng tháng</p>
        </div>
      </div>

      <div className="mpay-controls-wrapper">
        <div className="mpay-filters">
          <div className="mpay-input-wrapper">
            <svg className="mpay-input-icon" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <circle cx="11" cy="11" r="8"></circle>
              <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
            </svg>
            <input
              type="text"
              className="mpay-search-input"
              placeholder="Tìm kiếm theo tháng..."
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                setCurrentPage(1);
              }}
            />
          </div>
          
          <select 
            className="mpay-select"
            value={year}
            onChange={(e) => setYear(Number(e.target.value))}
          >
            {yearOptions.map(y => (
              <option key={y} value={y}>Năm {y}</option>
            ))}
          </select>
        </div>
      </div>

      <div className="mpay-table-wrapper">
        {loading ? (
          <div style={{ padding: '2rem', textAlign: 'center', color: 'var(--text-secondary)' }}>
            Đang tải dữ liệu...
          </div>
        ) : currentData.length === 0 && !error ? (
          <div style={{ padding: '2rem', textAlign: 'center', color: 'var(--text-secondary)' }}>
            Không có dữ liệu bảng lương cho năm {year}
          </div>
        ) : (
          <>
            <div style={{ overflowX: 'auto' }}>
              <table className="mpay-table">
                <thead>
                  <tr>
                    <th>Kỳ lương</th>
                    <th>Tổng thu nhập</th>
                    <th>Khoản trừ (BH & Thuế)</th>
                    <th>Thực lĩnh</th>
                    <th>Trạng thái</th>
                    <th style={{ textAlign: 'center' }}>Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {currentData.map(item => (
                    <tr key={item.idBangLuong}>
                      <td>Tháng {item.thang}/{item.nam}</td>
                      <td>{formatCurrency(item.tongThuNhap)}</td>
                      <td>{formatCurrency(item.khauTru + item.truThue + item.phat)}</td>
                      <td style={{ fontWeight: '600', color: '#7c3aed' }}>{formatCurrency(item.thucLinh)}</td>
                      <td>
                        <span className={`mpay-status ${
                          item.trangThai === 'DA_XAC_NHAN' ? 'success' : 
                          item.trangThai === 'YEU_CAU_XEM_XET' ? 'danger' : 'neutral'
                        }`}>
                          {item.trangThaiText}
                        </span>
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        <button 
                          className="mpay-btn-secondary" 
                          onClick={() => setSelectedPayslip(item)}
                        >
                          Xem chi tiết
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            
            {totalPages > 0 && (
              <div className="mpay-pagination">
                <div className="mpay-pagination-info">
                  Hiển thị {(currentPage - 1) * itemsPerPage + 1} - {Math.min(currentPage * itemsPerPage, filteredData.length)} / {filteredData.length} kết quả
                </div>
                <div className="mpay-pagination-controls">
                  <button 
                    className="mpay-btn-secondary"
                    disabled={currentPage === 1}
                    onClick={() => setCurrentPage(prev => prev - 1)}
                  >
                    Trước
                  </button>
                  <span style={{ padding: '0.5rem', fontSize: '0.875rem' }}>{currentPage} / {totalPages}</span>
                  <button 
                    className="mpay-btn-secondary"
                    disabled={currentPage === totalPages}
                    onClick={() => setCurrentPage(prev => prev + 1)}
                  >
                    Sau
                  </button>
                </div>
              </div>
            )}
          </>
        )}
      </div>

      {selectedPayslip && (
        <PayslipModal 
          payslip={selectedPayslip} 
          onClose={() => setSelectedPayslip(null)} 
          onPayslipUpdated={() => {
            refetch();
            setSelectedPayslip(null);
          }}
          onToast={(msg, type) => setToast({ message: msg, type })}
        />
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
