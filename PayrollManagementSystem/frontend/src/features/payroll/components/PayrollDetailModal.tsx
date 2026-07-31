import React from 'react';
import { PayrollListDto } from '../types/payroll.types';
import './PayrollDetailModal.css';

interface Props {
  payroll: PayrollListDto;
  onClose: () => void;
}

export const PayrollDetailModal: React.FC<Props> = ({ payroll, onClose }) => {
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
  };

  const luong3P = payroll.p1 * payroll.heSoP2 * payroll.heSoP3;

  const renderChiTietKhauTru = () => {
    if (!payroll.chiTietKhauTru) return null;
    try {
      const details = JSON.parse(payroll.chiTietKhauTru);
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

  return (
    <div className="payroll-modal-overlay" onClick={onClose}>
      <div className="payroll-modal-content" onClick={e => e.stopPropagation()}>
        <div className="payroll-modal-header">
          <h3>Chi tiết phiếu lương - {payroll.thang}/{payroll.nam}</h3>
          <button className="close-btn" onClick={onClose}>&times;</button>
        </div>
        
        <div className="payroll-modal-body">
          <div className="emp-info-card">
            <div className="emp-info-row">
              <span className="info-label">Nhân viên:</span>
              <span className="info-value">{payroll.tenNhanVien} ({payroll.cccdNhanVien})</span>
            </div>
            <div className="emp-info-row">
              <span className="info-label">Phòng ban:</span>
              <span className="info-value">{payroll.tenPhongBan}</span>
            </div>
            <div className="emp-info-row">
              <span className="info-label">Vị trí:</span>
              <span className="info-value">{payroll.tenChucVu}</span>
            </div>
            <div className="emp-info-row">
              <span className="info-label">Ngày công:</span>
              <span className="info-value">{payroll.ngayCongThucTe} / {payroll.ngayCongChuan}</span>
            </div>
            <div className="emp-info-row">
              <span className="info-label">Giờ công:</span>
              <span className="info-value">{payroll.gioCongThucTe} / {payroll.gioCongChuan}</span>
            </div>
          </div>

          <div className="salary-details-section">
            <h4>1. Thu nhập từ lương 3P</h4>
            
            <div className="salary-row">
              <div className="salary-label">
                P1 - Lương vị trí
              </div>
              <div className="salary-amount">{formatCurrency(payroll.p1)}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">
                P2 - Hệ số năng lực
              </div>
              <div className="salary-amount">x {payroll.heSoP2}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">
                P3 - Hệ số hiệu suất
              </div>
              <div className="salary-amount">x {payroll.heSoP3}</div>
            </div>

            <div className="salary-row sub-total">
              <div className="salary-label">
                Lương 3P
                <span className="salary-formula">({formatCurrency(payroll.p1)} x {payroll.heSoP2} x {payroll.heSoP3})</span>
              </div>
              <div className="salary-amount">{formatCurrency(luong3P)}</div>
            </div>

            <div className="salary-row sub-total">
              <div className="salary-label">
                Lương thời gian
                <span className="salary-formula">({formatCurrency(luong3P)} x {payroll.gioCongThucTe} giờ) / {payroll.gioCongChuan} giờ</span>
              </div>
              <div className="salary-amount amount-positive">{formatCurrency(payroll.luongThoiGian)}</div>
            </div>

            <h4 style={{ marginTop: '24px' }}>2. Phụ cấp & Thưởng / Phạt</h4>
            
            <div className="salary-row">
              <div className="salary-label">Phụ cấp</div>
              <div className="salary-amount amount-positive">+{formatCurrency(payroll.phuCap)}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">Lương tăng ca</div>
              <div className="salary-amount amount-positive">+{formatCurrency(payroll.tangCa)}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">Thưởng khác</div>
              <div className="salary-amount amount-positive">+{formatCurrency(payroll.thuong)}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">Phạt / Trừ lương</div>
              <div className="salary-amount amount-negative">-{formatCurrency(payroll.phat)}</div>
            </div>

            <h4 style={{ marginTop: '24px' }}>3. Khấu trừ (Bảo hiểm, Thuế)</h4>
            
            {renderChiTietKhauTru()}

            <div className="salary-row sub-total">
              <div className="salary-label">Tổng Khấu trừ</div>
              <div className="salary-amount amount-negative">-{formatCurrency(payroll.khauTru)}</div>
            </div>
            
            <div className="salary-row">
              <div className="salary-label">Thuế TNCN</div>
              <div className="salary-amount amount-negative">-{formatCurrency(payroll.truThue)}</div>
            </div>

            <div className="salary-row grand-total">
              <div className="salary-label">THỰC LĨNH</div>
              <div className="salary-amount">{formatCurrency(payroll.thucLinh)}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
