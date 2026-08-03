import React, { useState, useEffect, useRef } from 'react';
import { CreateUserCommand, RoleDto, EmployeeNoAccount } from '../types/user.types';
import { useEmployeesNoAccount } from '../hooks/useUsers';
import './UserManagement.css';

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: CreateUserCommand) => Promise<boolean>;
  roles: RoleDto[];
}

const initialFormState: CreateUserCommand = {
  tenTaiKhoan: '',
  matKhau: '',
  idVaiTro: '',
  cccd: ''
};

export const CreateUserModal: React.FC<Props> = ({ isOpen, onClose, onSubmit, roles }) => {
  const [formData, setFormData] = useState<CreateUserCommand>(initialFormState);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  // Custom Dropdown State
  const [searchTerm, setSearchTerm] = useState('');
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  const { employees, isLoading } = useEmployeesNoAccount(isOpen);

  useEffect(() => {
    if (!isOpen) {
      setFormData(initialFormState);
      setSearchTerm('');
      setIsDropdownOpen(false);
    }
  }, [isOpen]);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setIsDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const filteredEmployees = employees?.filter(emp => 
    `${emp.hoTen} - ${emp.cccd}`.toLowerCase().includes(searchTerm.toLowerCase())
  ) || [];

  const handleSelectEmployee = (emp: EmployeeNoAccount) => {
    setFormData({ ...formData, cccd: emp.cccd });
    setSearchTerm(`${emp.hoTen} - ${emp.cccd}`);
    setIsDropdownOpen(false);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.cccd || !formData.tenTaiKhoan || !formData.matKhau || !formData.idVaiTro) {
      alert('Vui lòng điền đầy đủ thông tin');
      return;
    }

    setIsSubmitting(true);
    const success = await onSubmit(formData);
    setIsSubmitting(false);
    
    if (success) onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="usr-modal-overlay">
      <div className="usr-modal">
        <div className="usr-modal-header">
          <h3 className="usr-modal-title">Cấp tài khoản mới</h3>
          <button className="usr-modal-close" onClick={onClose} disabled={isSubmitting}>
            &times;
          </button>
        </div>

        <div className="usr-modal-body">
          <form id="create-user-form" onSubmit={handleSubmit}>
            <div className="usr-form-group">
              <label className="usr-form-label">Nhân viên <span className="required">*</span></label>
              <div className="usr-dropdown-select-wrap" ref={dropdownRef}>
                <input
                  className="usr-form-select"
                  placeholder="-- Chọn nhân viên --"
                  value={searchTerm}
                  onChange={e => {
                    setSearchTerm(e.target.value);
                    setFormData({ ...formData, cccd: '' });
                    setIsDropdownOpen(true);
                  }}
                  onFocus={() => {
                    setSearchTerm('');
                    setFormData({ ...formData, cccd: '' });
                    setIsDropdownOpen(true);
                  }}
                  autoComplete="off"
                />
                {isDropdownOpen && (
                  <ul className="usr-dropdown-select-list custom-scrollbar">
                    {isLoading ? (
                      <li className="usr-empty-option">Đang tải...</li>
                    ) : filteredEmployees.length > 0 ? (
                      filteredEmployees.map(emp => (
                        <li 
                          key={emp.cccd}
                          className={formData.cccd === emp.cccd ? 'selected' : ''}
                          onClick={() => handleSelectEmployee(emp)}
                        >
                          {emp.hoTen} - {emp.cccd}
                        </li>
                      ))
                    ) : (
                      <li className="usr-empty-option">Không tìm thấy nhân viên</li>
                    )}
                  </ul>
                )}
              </div>
            </div>

            <div className="usr-form-group">
              <label className="usr-form-label">Tên đăng nhập <span className="required">*</span></label>
              <input 
                type="text"
                className="usr-form-input"
                placeholder="VD: nv_nguyenvana" 
                value={formData.tenTaiKhoan}
                onChange={(e) => setFormData({ ...formData, tenTaiKhoan: e.target.value })}
                required
              />
            </div>
            
            <div className="usr-form-group">
              <label className="usr-form-label">Mật khẩu khởi tạo <span className="required">*</span></label>
              <div className="usr-password-wrapper">
                <input 
                  type={showPassword ? "text" : "password"}
                  className="usr-form-input"
                  placeholder="Nhập mật khẩu an toàn" 
                  value={formData.matKhau}
                  onChange={(e) => setFormData({ ...formData, matKhau: e.target.value })}
                  required
                />
                <button
                  type="button"
                  className="usr-password-toggle"
                  onClick={() => setShowPassword(!showPassword)}
                  aria-label={showPassword ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
                  tabIndex={-1}
                >
                  {showPassword ? (
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="usr-icon-btn">
                      <path strokeLinecap="round" strokeLinejoin="round" d="M3.98 8.223A10.477 10.477 0 0 0 1.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.451 10.451 0 0 1 12 4.5c4.756 0 8.773 3.162 10.065 7.498a10.522 10.522 0 0 1-4.293 5.774M6.228 6.228 3 3m3.228 3.228 3.65 3.65m7.894 7.894L21 21m-3.228-3.228-3.65-3.65m0 0a3 3 0 1 0-4.243-4.243m4.242 4.242L9.88 9.88" />
                    </svg>
                  ) : (
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="usr-icon-btn">
                      <path strokeLinecap="round" strokeLinejoin="round" d="M2.036 12.322a1.012 1.012 0 0 1 0-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178Z" />
                      <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
                    </svg>
                  )}
                </button>
              </div>
            </div>

            <div className="usr-form-group">
              <label className="usr-form-label">Cấp quyền hạn (Vai trò) <span className="required">*</span></label>
              <select
                className="usr-form-select"
                value={formData.idVaiTro}
                onChange={(e) => setFormData({ ...formData, idVaiTro: e.target.value })}
                required
              >
                <option value="">-- Chọn vai trò --</option>
                {roles.map(role => (
                  <option key={role.idVaiTro} value={role.idVaiTro}>
                    {role.tenVaiTro}
                  </option>
                ))}
              </select>
            </div>
          </form>
        </div>

        <div className="usr-modal-footer">
          <button type="button" className="usr-btn usr-btn-secondary" onClick={onClose} disabled={isSubmitting}>
            Hủy bỏ
          </button>
          <button type="submit" form="create-user-form" className="usr-btn usr-btn-primary" disabled={isSubmitting}>
            {isSubmitting ? 'Đang tạo...' : 'Tạo tài khoản'}
          </button>
        </div>
      </div>
    </div>
  );
};