import React from 'react';
import { MyPayrollDto } from '../types/myPayroll.types';
import '../../payroll/components/PayrollDetailModal.css';

interface PayslipModalProps {
  payslip: MyPayrollDto;
  onClose: () => void;
}

export const PayslipModal: React.FC<PayslipModalProps> = ({ payslip, onClose }) => {
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
  };

  const luong3P = payslip.p1 * payslip.heSoP2 * payslip.heSoP3;

  const renderChiTietKhauTru = () => {
    if (!payslip.chiTietKhauTru) return null;
    try {
      const details = JSON.parse(payslip.chiTietKhauTru);
      if (!Array.isArray(details) || details.length === 0) return null;
      return (
        <div style={{ marginBottom: '8px' }}>
          {details.map((item: any, index: number) => (
            <div className="salary-row" key={index} style={{ paddingLeft: '16px', opacity: 0.8, fontSize: '0.95em' }}>
              <div className="salary-label">{item.ten}</div>
              <div className="salary-amount amount-negative">-{formatCurrency(item.soTien)}</div>
            </div>
          ))}
          <div style={{ borderBottom: '1px dashed var(--border-color)', margin: '8px 0' }}></div>
        </div>
      );
    } catch (e) {
      console.error("Error parsing chiTietKhauTru", e);
      return null;
    }
  };

  const renderChiTietThue = () => {
    if (!payslip.chiTietThue) return null;
    try {
      const details = JSON.parse(payslip.chiTietThue);
      
      return (
        <div style={{ marginBottom: '8px', paddingLeft: '16px', opacity: 0.9, fontSize: '0.9em' }}>
          <div className="salary-row" style={{ color: 'var(--text-secondary)' }}>
            <div className="salary-label">Thu nhập trước thuế:</div>
            <div>{formatCurrency(details.thuNhapTruocThue)}</div>
          </div>
          <div className="salary-row" style={{ color: 'var(--text-secondary)' }}>
            <div className="salary-label">Giảm trừ gia cảnh ({details.soNguoiPhuThuoc} NPT):</div>
            <div>-{formatCurrency(details.tongGiamTru)}</div>
          </div>
          <div className="salary-row" style={{ color: 'var(--text-secondary)' }}>
            <div className="salary-label">Thu nhập tính thuế:</div>
            <div>{formatCurrency(details.thuNhapTinhThue)}</div>
          </div>
          
          {details.chiTietBacThue && details.chiTietBacThue.length > 0 && (
            <div style={{ marginTop: '4px', paddingLeft: '8px', borderLeft: '2px solid var(--border-color)' }}>
              {details.chiTietBacThue.map((bac: any, index: number) => (
                <div className="salary-row" key={index} style={{ fontSize: '0.9em', color: 'var(--text-secondary)' }}>
                  <div className="salary-label">Bậc {bac.bac} ({bac.thueSuat}% của {formatCurrency(bac.thuNhapTinh)}):</div>
                  <div>-{formatCurrency(bac.soTien)}</div>
                </div>
              ))}
            </div>
          )}
          <div style={{ borderBottom: '1px dashed var(--border-color)', margin: '8px 0' }}></div>
        </div>
      );
    } catch (e) {
      console.error("Error parsing chiTietThue", e);
      return null;
    }
  };

  const handlePrint = () => {
    const originalTitle = document.title;
    document.title = `Phieu_Luong_T${payslip.thang}_${payslip.nam}_${payslip.tenNhanVien.replace(/\s+/g, '_')}`;
    window.print();
    document.title = originalTitle;
  };

  return (
    <div className="payroll-modal-overlay" onClick={onClose}>
      <div className="payroll-modal-content" onClick={e => e.stopPropagation()}>
        <div className="payroll-modal-header">
          <h3>Chi tiết phiếu lương - {payslip.thang}/{payslip.nam}</h3>
          <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
            <button 
              className="print-btn" 
              onClick={handlePrint}
              style={{
                background: 'rgba(255,255,255,0.2)',
                border: '1px solid rgba(255,255,255,0.3)',
                color: 'white',
                padding: '4px 12px',
                borderRadius: '6px',
                cursor: 'pointer',
                fontSize: '0.875rem',
                display: 'flex',
                alignItems: 'center',
                gap: '6px'
              }}
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <polyline points="6 9 6 2 18 2 18 9"></polyline>
                <path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2"></path>
                <rect x="6" y="14" width="12" height="8"></rect>
              </svg>
              In PDF
            </button>
            <button className="close-btn" onClick={onClose}>&times;</button>
          </div>
        </div>
        
        <div className="payroll-modal-body">
          <div className="emp-info-card">
            <div className="emp-info-row">
              <span className="info-label">Nhân viên:</span>
              <span className="info-value">{payslip.tenNhanVien} ({payslip.cccdNhanVien})</span>
            </div>
            <div className="emp-info-row">
              <span className="info-label">Phòng ban:</span>
              <span className="info-value">{payslip.tenPhongBan}</span>
            </div>
            <div className="emp-info-row">
              <span className="info-label">Vị trí:</span>
              <span className="info-value">{payslip.tenChucVu}</span>
            </div>
            <div className="emp-info-row">
              <span className="info-label">Ngày công:</span>
              <span className="info-value">{payslip.ngayCongThucTe} / {payslip.ngayCongChuan}</span>
            </div>
            <div className="emp-info-row">
              <span className="info-label">Giờ công:</span>
              <span className="info-value">{payslip.gioCongThucTe} / {payslip.gioCongChuan}</span>
            </div>
          </div>

          <div className="salary-details-section">
            <h4>1. Thu nhập từ lương 3P</h4>
            
            <div className="salary-row">
              <div className="salary-label">
                P1 - Lương vị trí
              </div>
              <div className="salary-amount">{formatCurrency(payslip.p1)}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">
                P2 - Hệ số năng lực
              </div>
              <div className="salary-amount">x {payslip.heSoP2}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">
                P3 - Hệ số hiệu suất
              </div>
              <div className="salary-amount">x {payslip.heSoP3}</div>
            </div>

            <div className="salary-row sub-total">
              <div className="salary-label">
                Lương 3P
                <span className="salary-formula">({formatCurrency(payslip.p1)} x {payslip.heSoP2} x {payslip.heSoP3})</span>
              </div>
              <div className="salary-amount">{formatCurrency(luong3P)}</div>
            </div>

            <div className="salary-row sub-total">
              <div className="salary-label">
                Lương thời gian
                <span className="salary-formula">({formatCurrency(luong3P)} x {payslip.gioCongThucTe} giờ) / {payslip.gioCongChuan} giờ</span>
              </div>
              <div className="salary-amount amount-positive">{formatCurrency(payslip.luongThoiGian)}</div>
            </div>

            <h4 style={{ marginTop: '24px' }}>2. Phụ cấp & Thưởng / Phạt</h4>
            
            <div className="salary-row">
              <div className="salary-label">Phụ cấp</div>
              <div className="salary-amount amount-positive">+{formatCurrency(payslip.phuCap)}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">Lương tăng ca</div>
              <div className="salary-amount amount-positive">+{formatCurrency(payslip.tangCa)}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">Thưởng khác</div>
              <div className="salary-amount amount-positive">+{formatCurrency(payslip.thuong)}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">Phạt / Trừ lương</div>
              <div className="salary-amount amount-negative">-{formatCurrency(payslip.phat)}</div>
            </div>

            <h4 style={{ marginTop: '24px' }}>3. Khấu trừ (Bảo hiểm, Thuế)</h4>
            
            {renderChiTietKhauTru()}

            <div className="salary-row sub-total">
              <div className="salary-label">Tổng Khấu trừ</div>
              <div className="salary-amount amount-negative">-{formatCurrency(payslip.khauTru)}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">Thuế TNCN</div>
              <div className="salary-amount amount-negative">-{formatCurrency(payslip.truThue)}</div>
            </div>
            {renderChiTietThue()}

            <div className="salary-row grand-total">
              <div className="salary-label">THỰC LĨNH</div>
              <div className="salary-amount">{formatCurrency(payslip.thucLinh)}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
