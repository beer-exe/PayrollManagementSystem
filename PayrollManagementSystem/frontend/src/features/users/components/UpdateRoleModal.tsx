import React, { useState, useEffect } from 'react';
import { Modal, Select } from 'antd';
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

  const handleSubmit = async () => {
    if (!user) return;
    setIsSubmitting(true);
    const success = await onSubmit(user.idTaiKhoan, { 
      idTaiKhoan: user.idTaiKhoan, 
      idVaiTroMoi: selectedRole 
    });
    setIsSubmitting(false);
    if (success) onClose();
  };

  return (
    <Modal
      title={<h3 className="user-modal-title">Cập nhật quyền hạn</h3>}
      open={isOpen}
      onCancel={onClose}
      onOk={handleSubmit}
      confirmLoading={isSubmitting}
      okText="Lưu thay đổi"
      cancelText="Hủy bỏ"
      okButtonProps={{ className: 'user-btn-primary !border-none !shadow-none' }}
      cancelButtonProps={{ className: '!rounded-lg' }}
      destroyOnClose
    >
      <div className="my-6">
        <div className="p-3 mb-4 rounded-lg bg-gray-50 border border-gray-100 dark:bg-gray-800 dark:border-gray-700">
          <p className="text-sm text-gray-500 dark:text-gray-400">
            Tài khoản đang thao tác:
          </p>
          <p className="text-base font-bold text-gray-900 dark:text-white mt-1">
            {user?.tenTaiKhoan} <span className="font-normal text-sm text-gray-500">({user?.hoTen})</span>
          </p>
        </div>
        
        <label className="user-form-label">Phân quyền mới</label>
        <Select
          size="large"
          className="w-full"
          value={selectedRole}
          onChange={(value) => setSelectedRole(value)}
          options={roles.map(role => ({
            value: role.idVaiTro,
            label: role.tenVaiTro
          }))}
        />
      </div>
    </Modal>
  );
};