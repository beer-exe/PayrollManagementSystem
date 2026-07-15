import React from 'react';
import './AuthLayout.css';
import { SessionExpiredModal } from '@/features/auth/components/SessionExpiredModal';

interface AuthLayoutProps {
  children: React.ReactNode;
}

export const AuthLayout: React.FC<AuthLayoutProps> = ({ children }) => {
  return (
    <div className="auth-layout-container">
      {/* Animated Background Shapes */}
      <div className="auth-bg-shapes">
        <div className="auth-shape auth-shape--1"></div>
        <div className="auth-shape auth-shape--2"></div>
        <div className="auth-shape auth-shape--3"></div>
      </div>
      
      <div className="auth-layout-content">
        <div className="auth-brand-section">
          <div className="auth-logo-wrapper">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" className="auth-logo-icon">
              <path d="M4.5 3.75a3 3 0 00-3 3v.75h21v-.75a3 3 0 00-3-3h-15z" />
              <path fillRule="evenodd" d="M22.5 9.75h-21v7.5a3 3 0 003 3h15a3 3 0 003-3v-7.5zm-18 3.75a.75.75 0 01.75-.75h6a.75.75 0 010 1.5h-6a.75.75 0 01-.75-.75zm.75 2.25a.75.75 0 000 1.5h3a.75.75 0 000-1.5h-3z" clipRule="evenodd" />
            </svg>
          </div>
          <h1 className="auth-brand-title">HRMS Portal</h1>
          <p className="auth-brand-subtitle">
            Giải pháp quản lý nhân sự và tiền lương toàn diện, giúp tối ưu hóa quy trình doanh nghiệp của bạn.
          </p>
        </div>
        
        <div className="auth-form-wrapper">
          {children}
        </div>
      </div>
      
      <SessionExpiredModal />
    </div>
  );
};