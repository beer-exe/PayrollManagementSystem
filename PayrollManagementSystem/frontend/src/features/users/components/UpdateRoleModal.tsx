import React, { useState, useEffect } from 'react';
import { UserDto, UpdateUserRoleCommand, RoleDto } from '../types/user.types';
import './UserManagement.css';

interface Props {
  user: UserDto | null;
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (id: string, data: UpdateUserRoleCommand) => Promise<boolean>;
  roles: RoleDto[];
}

export const UpdateRoleModal: React.FC<Props> = ({ user, isOpen, onClose, onSubmit, roles }) => {
  const [selectedRole, setSelectedRole] = useState<string>('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (user) setSelectedRole(user.idVaiTro);
  }, [user]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user || !selectedRole) {
      alert('Vui lòng chọn vai trò mới');
      return;
    }
    
    setIsSubmitting(true);
    const success = await onSubmit(user.idTaiKhoan, { 
      idTaiKhoan: user.idTaiKhoan, 
      idVaiTroMoi: selectedRole 
    });
    setIsSubmitting(false);
    if (success) onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="usr-modal-overlay">
      <div className="usr-modal">
        <div className="usr-modal-header">
          <h3 className="usr-modal-title">Cập nhật quyền hạn</h3>
          <button className="usr-modal-close" onClick={onClose} disabled={isSubmitting}>
            &times;
          </button>
        </div>

        <div className="usr-modal-body">
          <div style={{ padding: '1rem', marginBottom: '1.5rem', borderRadius: '8px', backgroundColor: 'var(--bg-hover)', border: '1px solid var(--border-color)' }}>
            <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', margin: '0 0 0.25rem 0' }}>
              Tài khoản đang thao tác:
            </p>
            <p style={{ fontSize: '1rem', fontWeight: 700, color: 'var(--text-primary)', margin: 0 }}>
              {user?.tenTaiKhoan} <span style={{ fontWeight: 400, fontSize: '0.875rem', color: 'var(--text-secondary)' }}>({user?.hoTen})</span>
            </p>
          </div>
          
          <form id="update-role-form" onSubmit={handleSubmit}>
            <div className="usr-form-group">
              <label className="usr-form-label">Phân quyền mới <span className="required">*</span></label>
              <select
                className="usr-form-select"
                value={selectedRole}
                onChange={(e) => setSelectedRole(e.target.value)}
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
          <button type="submit" form="update-role-form" className="usr-btn usr-btn-primary" disabled={isSubmitting}>
            {isSubmitting ? 'Đang lưu...' : 'Lưu thay đổi'}
          </button>
        </div>
      </div>
    </div>
  );
};