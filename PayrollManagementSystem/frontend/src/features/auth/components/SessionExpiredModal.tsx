import React from 'react';
import { useAuthStore } from '@/store/useAuthStore';
import './SessionExpiredModal.css';

export const SessionExpiredModal: React.FC = () => {
  const { isSessionExpired, logout } = useAuthStore();

  if (!isSessionExpired) return null;

  const handleRelogin = () => {
    logout();
    window.location.href = '/login';
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h3 className="modal-title">Phiên đăng nhập đã hết hạn</h3>
        <p className="modal-desc">
          Vì lý do bảo mật, vui lòng đăng nhập lại để tiếp tục sử dụng hệ thống.
        </p>
        <button onClick={handleRelogin} className="btn-primary mt-0">
          Đăng nhập lại
        </button>
      </div>
    </div>
  );
};