import React, { useState, useEffect } from 'react';
import { Modal, Input, Select } from 'antd';
import { CreateUserCommand, RoleDto } from '../types/user.types';

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: CreateUserCommand) => Promise<boolean>;
  roles: RoleDto[];
}

export const CreateUserModal: React.FC<Props> = ({ isOpen, onClose, onSubmit, roles }) => {
  const initialFormState: CreateUserCommand = {
    tenTaiKhoan: '',
    matKhau: '',
    idVaiTro: '',
    cccd: ''
  };

  const [formData, setFormData] = useState<CreateUserCommand>(initialFormState);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (!isOpen) {
      setFormData(initialFormState);
    }
  }, [isOpen]);

  const handleSubmit = async () => {
    setIsSubmitting(true);
    const success = await onSubmit(formData);
    setIsSubmitting(false);
    
    if (success) {
      onClose();
    }
  };

  return (
    <Modal
      title={<h3 className="text-lg font-bold text-gray-800">Thêm tài khoản mới</h3>}
      open={isOpen}
      onCancel={onClose}
      onOk={handleSubmit}
      confirmLoading={isSubmitting}
      okText="Tạo tài khoản"
      cancelText="Hủy"
      okButtonProps={{ className: 'bg-violet-600 hover:bg-violet-700' }}
    >
      <div className="space-y-4 my-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Căn cước công dân (Mã NV)</label>
          <Input 
            placeholder="Nhập CCCD của nhân viên" 
            value={formData.cccd}
            onChange={(e) => setFormData({ ...formData, cccd: e.target.value })}
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Tên đăng nhập</label>
          <Input 
            placeholder="Nhập tên đăng nhập" 
            value={formData.tenTaiKhoan}
            onChange={(e) => setFormData({ ...formData, tenTaiKhoan: e.target.value })}
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Mật khẩu</label>
          <Input.Password 
            placeholder="Nhập mật khẩu" 
            value={formData.matKhau}
            onChange={(e) => setFormData({ ...formData, matKhau: e.target.value })}
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Vai trò</label>
          <Select
            className="w-full"
            placeholder="Chọn vai trò"
            value={formData.idVaiTro || undefined}
            onChange={(value) => setFormData({ ...formData, idVaiTro: value })}
            options={roles.map(role => ({
              value: role.idVaiTro,
              label: role.tenVaiTro
            }))}
          />
        </div>
      </div>
    </Modal>
  );
};