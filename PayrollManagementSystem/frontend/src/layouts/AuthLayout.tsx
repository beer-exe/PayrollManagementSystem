import React from 'react';
import './AuthLayout.css';
import { SessionExpiredModal } from '@/features/auth/components/SessionExpiredModal';

interface AuthLayoutProps {
  children: React.ReactNode;
}

export const AuthLayout: React.FC<AuthLayoutProps> = ({ children }) => {
  return (
    <div className="auth-layout-container">
      <div className="auth-layout-left">
        {children}
      </div>
      <div className="auth-layout-right">
        <h1 className="auth-brand-title">HRMS Portal</h1>
        <p className="auth-brand-subtitle">
          Giải pháp quản lý nhân sự và tiền lương toàn diện, giúp tối ưu hóa quy trình doanh nghiệp của bạn.
        </p>
      </div>
      
      <SessionExpiredModal />
    </div>
  );
};