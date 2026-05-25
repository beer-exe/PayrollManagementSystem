import React, { useState, useEffect } from 'react';
import { Modal, Select } from 'antd';
import { UserDto, UpdateUserRoleCommand, RoleDto } from '../types/user.types';

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
      title={<h3 className="text-lg font-bold text-gray-800">Cập nhật quyền hạn</h3>}
      open={isOpen}
      onCancel={onClose}
      onOk={handleSubmit}
      confirmLoading={isSubmitting}
      okText="Lưu thay đổi"
      cancelText="Hủy"
      okButtonProps={{ className: 'bg-violet-600 hover:bg-violet-700' }}
    >
      <div className="my-6">
        <p className="text-sm text-gray-500 mb-4">
          Đang cập nhật quyền cho tài khoản: <strong className="text-gray-900">{user?.tenTaiKhoan}</strong>
        </p>
        <label className="block text-sm font-medium text-gray-700 mb-1">Vai trò mới</label>
        <Select
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