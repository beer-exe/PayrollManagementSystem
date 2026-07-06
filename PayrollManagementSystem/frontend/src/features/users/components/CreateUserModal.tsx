import React, { useState, useEffect } from 'react';
import { Modal, Input, Select, Spin } from 'antd';
import { CreateUserCommand, RoleDto } from '../types/user.types';
import { useEmployeesNoAccount } from '../hooks/useUsers';
import './UserManagement.css';

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

  const { employees, isLoading } = useEmployeesNoAccount(isOpen);

  useEffect(() => {
    if (!isOpen) {
      setFormData(initialFormState);
    }
  }, [isOpen]);

  const handleSubmit = async () => {
    setIsSubmitting(true);
    const success = await onSubmit(formData);
    setIsSubmitting(false);
    
    if (success) onClose();
  };

  return (
    <Modal
      title={<h3 className="user-modal-title">Cấp tài khoản mới</h3>}
      open={isOpen}
      onCancel={onClose}
      onOk={handleSubmit}
      confirmLoading={isSubmitting}
      okText="Tạo tài khoản"
      cancelText="Hủy bỏ"
      okButtonProps={{ className: 'user-btn-primary !border-none !shadow-none' }}
      cancelButtonProps={{ className: '!rounded-lg' }}
      destroyOnClose
    >
      <div className="space-y-4 mb-6">
        <div>
          <label className="user-form-label">Mã nhân viên (CCCD) <span className="text-red-500">*</span></label>
          <Select
            showSearch
            size="large"
            className="w-full"
            placeholder="Nhập CCCD hoặc Tên để tìm kiếm..."
            value={formData.cccd || undefined}
            onChange={(value) => setFormData({ ...formData, cccd: value })}
            filterOption={(input, option) => 
              (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
            }
            notFoundContent={isLoading ? <Spin size="small" /> : "Không tìm thấy nhân viên"}
            options={(employees ?? []).map((emp) => ({
              value: emp.cccd,
              label: `${emp.hoTen} - ${emp.cccd}`,
              cccd: emp.cccd
            }))}
          />
        </div>

        <div>
          <label className="user-form-label">Tên đăng nhập <span className="text-red-500">*</span></label>
          <Input 
            size="large"
            placeholder="VD: nv_nguyenvana" 
            value={formData.tenTaiKhoan}
            onChange={(e) => setFormData({ ...formData, tenTaiKhoan: e.target.value })}
          />
        </div>
        
        <div>
          <label className="user-form-label">Mật khẩu khởi tạo <span className="text-red-500">*</span></label>
          <Input.Password 
            size="large"
            placeholder="Nhập mật khẩu an toàn" 
            value={formData.matKhau}
            onChange={(e) => setFormData({ ...formData, matKhau: e.target.value })}
          />
        </div>

        <div>
          <label className="user-form-label">Cấp quyền hạn (Vai trò) <span className="text-red-500">*</span></label>
          <Select
            size="large"
            className="w-full"
            placeholder="-- Chọn vai trò --"
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