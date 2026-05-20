import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useLogin } from '../hooks/useLogin';
import './LoginForm.css';

export const LoginForm: React.FC = () => {
  const [tenTaiKhoan, setTenTaiKhoan] = useState('');
  const [matKhau, setMatKhau] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  
  const navigate = useNavigate();
  const { login, isLoading, error } = useLogin();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!tenTaiKhoan || !matKhau) return;
    
    const success = await login({ tenTaiKhoan, matKhau });
    if (success) {
      navigate('/dashboard'); 
    }
  };

  return (
    <div className="login-form-wrapper">
      <div className="login-form-header">
        <h2 className="login-form-title">Đăng nhập</h2>
        <p className="login-form-desc">Nhập tài khoản và mật khẩu để truy cập hệ thống.</p>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="input-group">
          <label htmlFor="tenTaiKhoan" className="input-label">Tên tài khoản</label>
          <input
            id="tenTaiKhoan"
            type="text"
            className="input-field"
            placeholder="Nhập tên tài khoản"
            value={tenTaiKhoan}
            onChange={(e) => setTenTaiKhoan(e.target.value)}
            disabled={isLoading}
            required
          />
        </div>

        <div className="input-group">
          <label htmlFor="matKhau" className="input-label">Mật khẩu</label>
          <div className="password-input-wrapper">
            <input
              id="matKhau"
              type={showPassword ? "text" : "password"}
              className="input-field pr-10"
              placeholder="••••••••"
              value={matKhau}
              onChange={(e) => setMatKhau(e.target.value)}
              disabled={isLoading}
              required
            />
            <button
              type="button"
              className="password-toggle-btn"
              onClick={() => setShowPassword(!showPassword)}
              aria-label={showPassword ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
              tabIndex={-1} // Bỏ qua khi người dùng dùng tab phím
            >
              {showPassword ? (
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M3.98 8.223A10.477 10.477 0 0 0 1.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.451 10.451 0 0 1 12 4.5c4.756 0 8.773 3.162 10.065 7.498a10.522 10.522 0 0 1-4.293 5.774M6.228 6.228 3 3m3.228 3.228 3.65 3.65m7.894 7.894L21 21m-3.228-3.228-3.65-3.65m0 0a3 3 0 1 0-4.243-4.243m4.242 4.242L9.88 9.88" />
                </svg>
              ) : (
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M2.036 12.322a1.012 1.012 0 0 1 0-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178Z" />
                  <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
                </svg>
              )}
            </button>
          </div>
        </div>

        {error && <div className="error-text">{error}</div>}

        <button type="submit" className="btn-primary" disabled={isLoading}>
          {isLoading ? 'Đang xử lý...' : 'Đăng nhập'}
        </button>
      </form>
    </div>
  );
};