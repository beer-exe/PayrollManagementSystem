import React, { useState } from 'react';
import { Modal, Input, Form, message } from 'antd';
import { departmentApi } from '../../api/departmentApi';
import './DepartmentModals.css';

interface CreateDeptModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const CreateDeptModal: React.FC<CreateDeptModalProps> = ({ isOpen, onClose, onSuccess }) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const onFinish = async (values: { idPb: string; tenPb: string }) => {
    setLoading(true);
    try {
      const res = await departmentApi.createDepartment(values);
      if (res.succeeded) {
        message.success('Tạo phòng ban thành công!');
        form.resetFields();
        onSuccess();
        onClose();
      }
    } catch (err) { const error = err as import('axios').AxiosError<{Message?: string}>;
      message.error(error.response?.data?.Message || 'Có lỗi xảy ra khi tạo phòng ban');
    } finally { 
      setLoading(false); 
    }
  };

  const handleCancel = () => {
    form.resetFields();
    onClose();
  };

  return (
    <Modal 
      title={<h3 className="dept-modal-title">Thêm Phòng Ban Mới</h3>} 
      open={isOpen} 
      onCancel={handleCancel} 
      onOk={() => form.submit()} 
      confirmLoading={loading}
      okText="Tạo mới"
      cancelText="Hủy bỏ"
      destroyOnClose
    >
      <Form 
        form={form} 
        onFinish={onFinish} 
        layout="vertical" 
        className="dept-form-layout"
      >
        <Form.Item 
          name="idPb" 
          label={<span className="dept-form-label">Mã Phòng Ban</span>} 
          rules={[
            { required: true, message: 'Vui lòng nhập mã!' },
            { max: 50, message: 'Mã không được vượt quá 50 ký tự!' }
          ]}
        >
          <Input placeholder="VD: PB_MARKETING" size="large" className="dept-form-input" />
        </Form.Item>
        
        <Form.Item 
          name="tenPb" 
          label={<span className="dept-form-label">Tên Phòng Ban</span>} 
          rules={[
            { required: true, message: 'Vui lòng nhập tên!' },
            { max: 100, message: 'Tên không được vượt quá 100 ký tự!' }
          ]}
        >
          <Input placeholder="VD: Phòng Marketing" size="large" className="dept-form-input" />
        </Form.Item>
      </Form>
    </Modal>
  );
};