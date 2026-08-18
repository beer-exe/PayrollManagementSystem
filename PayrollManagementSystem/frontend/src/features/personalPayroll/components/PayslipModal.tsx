import React from 'react';
import { MyPayrollDto } from '../types/myPayroll.types';
import '../../payroll/components/PayrollDetailModal.css';
import { personalPayrollApi } from '../api/personalPayrollApi';

interface PayslipModalProps {
  payslip: MyPayrollDto;
  onClose: () => void;
  onPayslipUpdated?: () => void;
  onToast?: (message: string, type: 'success' | 'error') => void;
}

export const PayslipModal: React.FC<PayslipModalProps> = ({ payslip, onClose, onPayslipUpdated, onToast }) => {
  const [isSubmitting, setIsSubmitting] = React.useState(false);
  const [showReviewPrompt, setShowReviewPrompt] = React.useState(false);
  const [reviewReason, setReviewReason] = React.useState('');
  const [errorMsg, setErrorMsg] = React.useState('');

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
  };

  const handleConfirm = async () => {
    if (!window.confirm('Bạn xác nhận phiếu lương này là chính xác? Hành động này không thể hoàn tác.')) return;
    try {
      setIsSubmitting(true);
      setErrorMsg('');
      await personalPayrollApi.confirmPayslip(payslip.idBangLuong);
      if (onToast) onToast('Xác nhận bảng lương thành công!', 'success');
      if (onPayslipUpdated) onPayslipUpdated();
      onClose();
    } catch (err: any) {
      const msg = err.response?.data?.message || err.message || 'Có lỗi xảy ra khi xác nhận.';
      if (onToast) onToast(msg, 'error');
      setIsSubmitting(false);
    }
  };

  const handleRequestReview = async () => {
    if (!reviewReason.trim()) {
      if (onToast) onToast('Vui lòng nhập lý do yêu cầu xem xét.', 'error');
      setErrorMsg('Vui lòng nhập lý do yêu cầu xem xét.');
      return;
    }
    try {
      setIsSubmitting(true);
      setErrorMsg('');
      await personalPayrollApi.requestReviewPayslip(payslip.idBangLuong, reviewReason);
      if (onToast) onToast('Gửi yêu cầu xem xét thành công!', 'success');
      if (onPayslipUpdated) onPayslipUpdated();
      onClose();
    } catch (err: any) {
      const msg = err.response?.data?.message || err.message || 'Có lỗi xảy ra khi yêu cầu xem xét.';
      if (onToast) onToast(msg, 'error');
      setIsSubmitting(false);
    }
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

  const renderStatusBadge = () => {
    let statusClass = 'neutral';
    if (payslip.trangThai === 'DA_XAC_NHAN') {
      statusClass = 'success';
    } else if (payslip.trangThai === 'YEU_CAU_XEM_XET') {
      statusClass = 'danger';
    }
    return (
      <span className={`mpay-status ${statusClass}`} style={{ marginLeft: '12px' }}>
        {payslip.trangThaiText}
      </span>
    );
  };

  return (
    <div className="payroll-modal-overlay" onClick={onClose}>
      <div className="payroll-modal-content" onClick={e => e.stopPropagation()}>
        <div className="payroll-modal-header" style={{ flexWrap: 'wrap' }}>
          <div style={{ display: 'flex', alignItems: 'center' }}>
            <h3 style={{ margin: 0 }}>Chi tiết phiếu lương - {payslip.thang}/{payslip.nam}</h3>
            {renderStatusBadge()}
          </div>
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
          {payslip.trangThai === 'YEU_CAU_XEM_XET' && (
            <div className="prl-review-box">
              <h4 className="prl-review-box-header">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z"></path><path d="M12 9v4"></path><path d="M12 17h.01"></path></svg>
                Yêu cầu xem xét của bạn đang được xử lý
              </h4>
              <p className="prl-review-box-reason">
                Lý do: <span>{payslip.lyDoKhieuNai}</span>
              </p>
            </div>
          )}

          {payslip.phanHoiKhieuNai && (
            <div className="prl-feedback-box">
              <h4 className="prl-feedback-box-header">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path></svg>
                Phản hồi từ phòng Hành chính Nhân sự
              </h4>
              <p style={{ margin: 0, fontSize: '0.9em', fontWeight: 500 }}>
                <span style={{ fontWeight: 400 }}>{payslip.phanHoiKhieuNai}</span>
              </p>
            </div>
          )}
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

            {/*
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
            */}

            <h4 style={{ marginTop: '24px' }}>2. Khấu trừ (Bảo hiểm, Thuế)</h4>

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
        
        {showReviewPrompt && payslip.trangThai === 'CHUA_XAC_NHAN' && (
          <div style={{ padding: '16px 24px', backgroundColor: 'rgba(245, 158, 11, 0.05)', borderTop: '1px solid rgba(245, 158, 11, 0.3)' }}>
            <p style={{ margin: '0 0 8px 0', fontWeight: 600, color: '#f59e0b' }}>
              Nhập lý do yêu cầu xem xét lại phiếu lương:
            </p>
            <textarea
              style={{ width: '100%', padding: '8px', borderRadius: '4px', border: '1px solid rgba(245, 158, 11, 0.3)', minHeight: '60px', backgroundColor: 'var(--bg-main)', color: 'var(--text-primary)' }}
              value={reviewReason}
              onChange={e => setReviewReason(e.target.value)}
              placeholder="Ví dụ: Thiếu giờ tăng ca ngày 15/10..."
            />
            {errorMsg && <p style={{ color: '#ef4444', fontSize: '0.875rem', margin: '4px 0' }}>{errorMsg}</p>}
            <div style={{ marginTop: '8px', display: 'flex', justifyContent: 'flex-end', gap: '8px' }}>
              <button 
                onClick={() => setShowReviewPrompt(false)}
                style={{ padding: '6px 12px', background: 'var(--bg-main)', color: 'var(--text-primary)', border: '1px solid var(--border-color)', borderRadius: '4px', cursor: 'pointer' }}
              >Hủy</button>
              <button 
                onClick={handleRequestReview}
                disabled={isSubmitting || !reviewReason.trim()}
                style={{ padding: '6px 12px', background: '#d97706', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}
              >Gửi yêu cầu</button>
            </div>
          </div>
        )}

        {payslip.trangThai === 'YEU_CAU_XEM_XET' && (
          <div style={{ padding: '12px 24px', backgroundColor: 'rgba(239, 68, 68, 0.05)', borderTop: '1px solid rgba(239, 68, 68, 0.3)', color: '#ef4444' }}>
            <strong>Lý do khiếu nại của bạn:</strong> {payslip.lyDoKhieuNai}
          </div>
        )}

        {payslip.trangThai === 'CHUA_XAC_NHAN' && !showReviewPrompt && (
          <div style={{ 
            padding: '16px 24px', 
            borderTop: '1px solid var(--border-color)', 
            display: 'flex', 
            justifyContent: 'flex-end', 
            gap: '12px', 
            background: 'var(--bg-main)',
            borderBottomLeftRadius: '12px',
            borderBottomRightRadius: '12px'
          }}>
            <button
              className="print-btn"
              onClick={() => setShowReviewPrompt(!showReviewPrompt)}
              style={{ background: '#f59e0b', borderColor: '#d97706', padding: '8px 16px', fontSize: '0.95rem' }}
              disabled={isSubmitting}
            >
              ⚠️ Yêu cầu xem xét
            </button>
            <button
              className="print-btn"
              onClick={handleConfirm}
              style={{ background: '#10b981', borderColor: '#059669', padding: '8px 16px', fontSize: '0.95rem' }}
              disabled={isSubmitting}
            >
              ✅ Xác nhận đúng
            </button>
          </div>
        )}
      </div>
    </div>
  );
};
