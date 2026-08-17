import React, { useState } from 'react';
import './ReopenPayrollModal.css';

interface ReopenPayrollModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: (lyDo: string) => Promise<void>;
  thang: number;
  nam: number;
  loading: boolean;
}

export const ReopenPayrollModal: React.FC<ReopenPayrollModalProps> = ({
  isOpen,
  onClose,
  onConfirm,
  thang,
  nam,
  loading
}) => {
  const [lyDo, setLyDo] = useState('');
  const [error, setError] = useState('');

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!lyDo.trim()) {
      setError('Vui lòng nhập lý do mở chốt kỳ lương.');
      return;
    }
    setError('');
    await onConfirm(lyDo.trim());
  };

  return (
    <div className="prl-ro-overlay" onClick={onClose}>
      <div className="prl-ro-modal" onClick={(e) => e.stopPropagation()}>
        <div className="prl-ro-header">
          <div className="prl-ro-header-title">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 22, height: 22 }}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 10.5V6.75a4.5 4.5 0 1 1 9 0v3.75M3.75 21.75h10.5a2.25 2.25 0 0 0 2.25-2.25v-6.75a2.25 2.25 0 0 0-2.25-2.25H3.75a2.25 2.25 0 0 0-2.25 2.25v6.75a2.25 2.25 0 0 0 2.25 2.25Z" />
            </svg>
            <h3>Mở chốt bảng lương tháng {thang}/{nam}</h3>
          </div>
          <button className="prl-ro-header-close" onClick={onClose} disabled={loading} title="Đóng">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 20, height: 20 }}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="prl-ro-body">
            <div className="prl-ro-warning-box">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: 22, height: 22, flexShrink: 0, marginTop: 1 }}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z" />
              </svg>
              <div>
                <strong>Lưu ý quan trọng:</strong> Sau khi mở chốt, trạng thái kỳ lương sẽ trở về <em>Chưa chốt</em>. Bộ phận HR có thể chạy tính lại bảng lương với dữ liệu công mới nhất.
              </div>
            </div>

            <div className="prl-ro-form-group">
              <label className="prl-ro-form-label" htmlFor="ro-lydo">
                Lý do mở chốt <span className="required">*</span>
              </label>
              <textarea
                id="ro-lydo"
                className="prl-ro-textarea"
                placeholder="Ví dụ: Chốt sớm giữa tháng, cần mở để tính bù công nửa tháng còn lại cho nhân viên..."
                value={lyDo}
                onChange={(e) => {
                  setLyDo(e.target.value);
                  if (error) setError('');
                }}
                disabled={loading}
                autoFocus
              />
              {error && <span style={{ color: 'var(--danger, #ef4444)', fontSize: '0.8rem' }}>{error}</span>}
            </div>
          </div>

          <div className="prl-ro-footer">
            <button
              type="button"
              className="prl-ro-btn prl-ro-btn-secondary"
              onClick={onClose}
              disabled={loading}
            >
              Hủy bỏ
            </button>
            <button
              type="submit"
              className="prl-ro-btn prl-ro-btn-warning"
              disabled={loading}
            >
              {loading ? (
                <>
                  <div className="prl-spinner" style={{ width: 14, height: 14, borderRightColor: 'transparent', borderTopColor: '#fff', borderLeftColor: '#fff', borderBottomColor: '#fff' }} />
                  Đang mở chốt...
                </>
              ) : (
                <>
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 16, height: 16 }}>
                    <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 10.5V6.75a4.5 4.5 0 1 1 9 0v3.75M3.75 21.75h10.5a2.25 2.25 0 0 0 2.25-2.25v-6.75a2.25 2.25 0 0 0-2.25-2.25H3.75a2.25 2.25 0 0 0-2.25 2.25v6.75a2.25 2.25 0 0 0 2.25 2.25Z" />
                  </svg>
                  Xác nhận mở chốt
                </>
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
