import React, { useEffect, useState } from 'react';
import { Table, Button, Input, Modal, Form, Space, Tag, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, SettingOutlined } from '@ant-design/icons';
import { useJobGrades } from '../hooks/useJobGrades';
import { JobGrade } from '../types/jobGrade.types';
import { JobGradeSalaryStepDrawer } from './JobGradeSalaryStepDrawer';

export const NgachLuongManagement: React.FC = () => {
  const { jobGrades, loading, fetchJobGrades, createJobGrade, updateJobGrade, deleteJobGrade } = useJobGrades();

  useEffect(() => {
    fetchJobGrades();
  }, [fetchJobGrades]);

  const [isModalVisible, setIsModalVisible] = useState(false);
  const [editingGrade, setEditingGrade] = useState<JobGrade | null>(null);
  const [form] = Form.useForm();
  const [submitLoading, setSubmitLoading] = useState(false);

  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selectedGradeId, setSelectedGradeId] = useState<string | null>(null);
  const [selectedGradeName, setSelectedGradeName] = useState<string>('');

  const handleAdd = () => {
    setEditingGrade(null);
    form.resetFields();
    form.setFieldsValue({ trangThai: 1 });
    setIsModalVisible(true);
  };

  const handleEdit = (record: JobGrade) => {
    setEditingGrade(record);
    form.setFieldsValue({
      tenNgachLuong: record.tenNgachLuong,
      moTa: record.moTa,
      trangThai: record.trangThai
    });
    setIsModalVisible(true);
  };

  const handleDelete = async (id: string) => {
    const success = await deleteJobGrade(id);
    if (success) {
      fetchJobGrades();
    }
  };

  const handleModalOk = async () => {
    try {
      const values = await form.validateFields();
      setSubmitLoading(true);
      
      let success = false;
      if (editingGrade) {
        success = await updateJobGrade({
          idNgachLuong: editingGrade.idNgachLuong,
          ...values
        });
      } else {
        success = await createJobGrade(values);
      }
      
      if (success) {
        setIsModalVisible(false);
        fetchJobGrades();
      }
    } catch (error) {
      console.error('Validation failed:', error);
    } finally {
      setSubmitLoading(false);
    }
  };

  const openDrawer = (record: JobGrade) => {
    setSelectedGradeId(record.idNgachLuong);
    setSelectedGradeName(record.tenNgachLuong);
    setDrawerOpen(true);
  };

  const columns = [
    {
      title: 'Mã Ngạch',
      dataIndex: 'idNgachLuong',
      key: 'idNgachLuong',
      width: 150,
      render: (text: string) => <span className="font-mono">{text}</span>,
    },
    {
      title: 'Tên Ngạch',
      dataIndex: 'tenNgachLuong',
      key: 'tenNgachLuong',
      className: 'font-semibold text-gray-800 dark:text-white',
    },
    {
      title: 'Mô tả',
      dataIndex: 'moTa',
      key: 'moTa',
    },
    {
      title: 'Trạng thái',
      dataIndex: 'trangThai',
      key: 'trangThai',
      render: (status: number, record: JobGrade) => {
        return status === 1 ? (
          <Tag color="success" className="rounded-full px-3">{record.tenTrangThai}</Tag>
        ) : (
          <Tag color="default" className="rounded-full px-3 text-gray-500">{record.tenTrangThai}</Tag>
        );
      },
    },
    {
      title: 'Thao tác',
      key: 'actions',
      align: 'right' as const,
      width: 300,
      render: (_: any, record: JobGrade) => (
        <Space size="small">
          <Button 
            type="text" 
            icon={<SettingOutlined />} 
            className="text-blue-600 hover:bg-blue-50"
            onClick={() => openDrawer(record)}
          >
            Bậc lương
          </Button>
          <Button 
            type="text" 
            icon={<EditOutlined />} 
            className="text-violet-600 hover:bg-violet-50"
            onClick={() => handleEdit(record)} 
          >
            Sửa
          </Button>
          <Popconfirm
            title="Xóa ngạch lương?"
            description="Bạn có chắc chắn muốn xóa ngạch lương này không?"
            onConfirm={() => handleDelete(record.idNgachLuong)}
            okText="Đồng ý"
            cancelText="Hủy"
          >
            <Button type="text" danger icon={<DeleteOutlined />}>Xóa</Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div className="p-4 sm:p-6 h-full flex flex-col bg-gray-50/50 dark:bg-gray-900 min-w-0 overflow-hidden">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6 gap-4">
        <div>
          <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
            Danh Mục Ngạch Lương
          </h2>
          <p className="text-sm text-gray-500 mt-1">
            Quản lý các ngạch lương và bậc lương tương ứng
          </p>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={handleAdd}
          className="bg-violet-600 hover:bg-violet-700 h-10 px-5 rounded-lg shadow-sm"
        >
          Thêm Ngạch Lương
        </Button>
      </div>

      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-100 flex flex-col flex-1 overflow-hidden min-h-0">
        <div className="grid grid-cols-1 w-full flex-1 min-h-0">
          <Table
            columns={columns}
            dataSource={jobGrades}
            rowKey="idNgachLuong"
            loading={loading}
            scroll={{ x: 1050 }}
            pagination={{ 
              pageSize: 10,
              className: "px-4 py-3 m-0 border-t border-gray-100 dark:border-gray-700 bg-gray-50 dark:bg-gray-800/50",
            }}
          />
        </div>
      </div>

      <Modal
        title={
          <h3 className="text-xl font-bold text-gray-900 pb-2 border-b border-gray-100 mb-4">
            {editingGrade ? 'Chỉnh sửa ngạch lương' : 'Thêm mới ngạch lương'}
          </h3>
        }
        open={isModalVisible}
        onOk={handleModalOk}
        onCancel={() => setIsModalVisible(false)}
        confirmLoading={submitLoading}
        okText="Lưu lại"
        cancelText="Hủy bỏ"
        destroyOnClose
        okButtonProps={{ className: "bg-violet-600 hover:bg-violet-700 rounded-lg" }}
        cancelButtonProps={{ className: "rounded-lg" }}
      >
        <Form form={form} layout="vertical" className="mt-2">
          <Form.Item
            name="tenNgachLuong"
            label={<span className="font-semibold text-gray-700">Tên Ngạch Lương</span>}
            rules={[{ required: true, message: 'Vui lòng nhập tên ngạch lương' }]}
          >
            <Input placeholder="VD: G1, G2, Chuyên viên chính..." size="large" className="rounded-lg" />
          </Form.Item>
          <Form.Item 
            name="moTa" 
            label={<span className="font-semibold text-gray-700">Mô tả</span>}
          >
            <Input.TextArea rows={4} placeholder="Mô tả chi tiết về ngạch lương này" className="rounded-lg" />
          </Form.Item>
          {editingGrade && (
            <Form.Item 
              name="trangThai" 
              label={<span className="font-semibold text-gray-700">Trạng thái</span>} 
              rules={[{ required: true }]}
            >
              <select className="w-full h-10 px-3 border border-gray-300 rounded-lg bg-white">
                <option value={1}>Đang sử dụng</option>
                <option value={0}>Ngừng sử dụng</option>
              </select>
            </Form.Item>
          )}
        </Form>
      </Modal>

      <JobGradeSalaryStepDrawer
        jobGradeId={selectedGradeId}
        jobGradeName={selectedGradeName}
        isOpen={drawerOpen}
        onClose={() => setDrawerOpen(false)}
      />
    </div>
  );
};
