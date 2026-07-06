import React, { useEffect, useState } from 'react';
import { Table, Button, Modal, Form, InputNumber, Input, Space, Tag } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useMucQuyDoi } from '../hooks/useMucQuyDoi';
import { MucQuyDoiDto } from '../types/mucQuyDoi.types';

export const MucQuyDoiManagement: React.FC = () => {
  const { data, loading, fetchQuyDoi, createQuyDoi, updateQuyDoi, deleteQuyDoi } = useMucQuyDoi();
  
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [editingItem, setEditingItem] = useState<MucQuyDoiDto | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchQuyDoi();
  }, [fetchQuyDoi]);

  const handleOpenModal = (record?: MucQuyDoiDto) => {
    setEditingItem(record || null);
    if (record) {
      form.setFieldsValue(record);
    } else {
      form.resetFields();
    }
    setIsModalVisible(true);
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      let success = false;
      if (editingItem) {
        success = await updateQuyDoi(editingItem.idQuyDoi, values);
      } else {
        success = await createQuyDoi(values);
      }

      if (success) {
        setIsModalVisible(false);
        fetchQuyDoi();
      }
    } catch (info) {
      console.log('Validate Failed:', info);
    }
  };

  const handleDelete = (record: MucQuyDoiDto) => {
    Modal.confirm({
      title: 'Xác nhận xóa',
      content: `Bạn có chắc muốn xóa xếp loại "${record.xepLoai}" không?`,
      okText: 'Xóa',
      okType: 'danger',
      cancelText: 'Hủy',
      onOk: async () => {
        const success = await deleteQuyDoi(record.idQuyDoi);
        if (success) {
          fetchQuyDoi();
        }
      }
    });
  };

  const columns = [
    { 
      title: 'Xếp loại', 
      dataIndex: 'xepLoai', 
      key: 'xepLoai',
      render: (text: string) => {
        let color = 'default';
        if (text.includes('A')) color = 'success';
        else if (text.includes('B')) color = 'processing';
        else if (text.includes('C')) color = 'warning';
        else if (text.includes('D')) color = 'error';
        return <Tag color={color} className="font-bold px-3 py-0.5 rounded-full text-sm">{text}</Tag>;
      }
    },
    { 
      title: 'Điểm tối thiểu', 
      dataIndex: 'diemToiThieu', 
      key: 'diemToiThieu',
      align: 'right' as const,
      render: (val: number) => <span className="font-semibold">{val.toLocaleString('vi-VN')}</span>
    },
    { 
      title: 'Điểm tối đa', 
      dataIndex: 'diemToiDa', 
      key: 'diemToiDa',
      align: 'right' as const,
      render: (val: number) => <span className="font-semibold text-violet-600">{val.toLocaleString('vi-VN')}</span>
    },
    { 
      title: 'Hệ số P2', 
      dataIndex: 'heSoP2', 
      key: 'heSoP2',
      align: 'center' as const,
      render: (val: number) => <span className="font-bold text-emerald-600">{val.toLocaleString('vi-VN')}</span>
    },
    {
      title: 'Hành Động',
      key: 'actions',
      align: 'right' as const,
      render: (_: unknown, record: MucQuyDoiDto) => (
        <Space size="middle">
          <Button 
            type="text" 
            icon={<EditOutlined />} 
            className="text-violet-600 hover:bg-violet-50"
            onClick={() => handleOpenModal(record)}
          >
            Sửa
          </Button>
          <Button 
            type="text" 
            danger
            icon={<DeleteOutlined />} 
            className="hover:bg-red-50"
            onClick={() => handleDelete(record)}
          >
            Xóa
          </Button>
        </Space>
      )
    }
  ];

  return (
    <div className="p-4 sm:p-6 h-full flex flex-col bg-gray-50/50 dark:bg-gray-900 min-w-0 overflow-hidden">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6 gap-4">
        <div>
          <h2 className="text-2xl font-bold text-gray-900 dark:text-white">Cấu Hình Mức Quy Đổi P2</h2>
          <p className="text-sm text-gray-500 mt-1">Quản lý các dải điểm đánh giá năng lực và hệ số lương P2 tương ứng</p>
        </div>
        <Button 
          type="primary" 
          icon={<PlusOutlined />} 
          onClick={() => handleOpenModal()}
          className="bg-violet-600 hover:bg-violet-700 h-10 px-5 rounded-lg shadow-sm"
        >
          Thêm Mức Quy Đổi
        </Button>
      </div>

      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-100 flex flex-col flex-1 overflow-hidden min-h-0">
        <div className="grid grid-cols-1 w-full flex-1 min-h-0">
          <Table 
            columns={columns} 
            dataSource={data} 
            rowKey="idQuyDoi" 
            loading={loading}
            pagination={false}
            scroll={{ y: 'max-content', x: 800 }}
            className="h-full"
          />
        </div>
      </div>

      <Modal
        title={
          <h3 className="text-xl font-bold text-gray-900 pb-2 border-b border-gray-100 mb-4">
            {editingItem ? "Cập Nhật Mức Quy Đổi" : "Thêm Mới Mức Quy Đổi"}
          </h3>
        }
        open={isModalVisible}
        onCancel={() => setIsModalVisible(false)}
        onOk={handleSubmit}
        okText="Lưu lại"
        cancelText="Hủy bỏ"
        destroyOnClose
        okButtonProps={{ className: "bg-violet-600 hover:bg-violet-700 rounded-lg" }}
        cancelButtonProps={{ className: "rounded-lg" }}
        width={500}
      >
        <Form form={form} layout="vertical" className="mt-2">
          <Form.Item 
            name="xepLoai" 
            label={<span className="font-semibold text-gray-700">Xếp loại (VD: A+, A, B)</span>} 
            rules={[
              { required: true, message: 'Vui lòng nhập xếp loại!' },
              { max: 50, message: 'Không vượt quá 50 ký tự!' }
            ]}
          >
            <Input size="large" placeholder="Nhập tên xếp loại..." className="rounded-lg" />
          </Form.Item>
          
          <div className="flex gap-4">
            <Form.Item 
              name="diemToiThieu" 
              label={<span className="font-semibold text-gray-700">Điểm tối thiểu</span>} 
              className="flex-1"
              rules={[
                { required: true, message: 'Bắt buộc nhập!' },
                { type: 'number', min: 0, message: 'Phải >= 0' }
              ]}
            >
              <InputNumber size="large" className="w-full rounded-lg" step={0.1} placeholder="0.0" />
            </Form.Item>
            
            <Form.Item 
              name="diemToiDa" 
              label={<span className="font-semibold text-gray-700">Điểm tối đa</span>} 
              className="flex-1"
              dependencies={['diemToiThieu']}
              rules={[
                { required: true, message: 'Bắt buộc nhập!' },
                ({ getFieldValue }) => ({
                  validator(_, value) {
                    const min = getFieldValue('diemToiThieu');
                    if (value == null || min == null || value > min) {
                      return Promise.resolve();
                    }
                    return Promise.reject(new Error('Điểm tối đa phải lớn hơn điểm tối thiểu!'));
                  },
                }),
              ]}
            >
              <InputNumber size="large" className="w-full rounded-lg" step={0.1} placeholder="10.0" />
            </Form.Item>
          </div>

          <Form.Item 
            name="heSoP2" 
            label={<span className="font-semibold text-gray-700">Hệ số P2</span>} 
            rules={[
              { required: true, message: 'Vui lòng nhập hệ số P2!' },
              { type: 'number', min: 0, message: 'Hệ số phải >= 0' }
            ]}
          >
            <InputNumber size="large" className="w-full rounded-lg" step={0.01} placeholder="1.0" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};
