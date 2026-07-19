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
              <input 
                type="password"
                className="usr-form-input"
                placeholder="Nhập mật khẩu an toàn" 
                value={formData.matKhau}
                onChange={(e) => setFormData({ ...formData, matKhau: e.target.value })}
                required
              />
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