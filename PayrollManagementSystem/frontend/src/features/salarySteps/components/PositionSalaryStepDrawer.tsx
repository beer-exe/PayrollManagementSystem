import React, { useEffect, useState } from 'react';
import { Drawer, Table, Button, Modal, Form, Input, InputNumber, DatePicker, message, Tag, Space } from 'antd';
import { PlusOutlined, HistoryOutlined, DeleteOutlined, EditOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { salaryStepApi } from '../api/salaryStepApi';
import { SalaryStepDto } from '../types/salaryStep.types';

interface Props {
  positionId: string | null;
  positionName: string;
  isOpen: boolean;
  onClose: () => void;
}

export const PositionSalaryStepDrawer: React.FC<Props> = ({ positionId, positionName, isOpen, onClose }) => {
  const [activeSteps, setActiveSteps] = useState<SalaryStepDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [historyData, setHistoryData] = useState<SalaryStepDto[]>([]);
  const [historyModalOpen, setHistoryModalOpen] = useState(false);
  const [formModalOpen, setFormModalOpen] = useState(false);
  const [isUpdatingVersion, setIsUpdatingVersion] = useState(false);
  
  const [form] = Form.useForm();

  const fetchActiveSteps = async () => {
    setLoading(true);
    try {
      const res = await salaryStepApi.getActive(positionId!);
      if (res.succeeded) setActiveSteps(res.data);
    } finally { setLoading(false); }
  };

  useEffect(() => {
    if (isOpen && positionId) fetchActiveSteps();
  }, [isOpen, positionId]);

  const handleOpenCreate = () => {
    setIsUpdatingVersion(false);
    form.resetFields();
    setFormModalOpen(true);
  };

  const handleOpenUpdateVersion = (record: SalaryStepDto) => {
    setIsUpdatingVersion(true);
    form.setFieldsValue({
      stepName: record.stepName,
      newP1Salary: record.p1Salary,
      newEffectiveDate: null
    });
    setFormModalOpen(true);
  };

  const viewHistory = async (stepName: string) => {
    try {
      const res = await salaryStepApi.getHistory(positionId!, stepName);
      if (res.succeeded) {
        setHistoryData(res.data);
        setHistoryModalOpen(true);
      }
    } catch (error) { message.error("Lỗi tải dữ liệu lịch sử"); }
  };

  const handleDelete = (stepName: string) => {
    Modal.confirm({
      title: 'Cảnh báo nguy hiểm',
      content: `Xóa toàn bộ dữ liệu của ${stepName}? Sẽ bị chặn nếu đã áp dụng cho nhân sự.`,
      okType: 'danger',
      onOk: async () => {
        try {
          const res = await salaryStepApi.delete(positionId!, stepName);
          if (res.succeeded) {
            message.success("Đã xóa thành công");
            fetchActiveSteps();
          }
        } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>; message.error(err.response?.data?.Message || "Xóa thất bại"); }
      }
    });
  };

  const handleFormSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (isUpdatingVersion) {
        await salaryStepApi.updateVersion({
          positionId: positionId!,
          stepName: values.stepName,
          newP1Salary: values.newP1Salary,
          newEffectiveDate: values.newEffectiveDate.format('YYYY-MM-DD')
        });
        message.success("Cập nhật phiên bản lương mới thành công!");
      } else {
        await salaryStepApi.create({
          positionId: positionId!,
          stepName: values.stepName,
          p1Salary: values.p1Salary,
          effectiveDate: values.effectiveDate.format('YYYY-MM-DD')
        });
        message.success("Tạo bậc lương thành công!");
      }
      setFormModalOpen(false);
      fetchActiveSteps();
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      if (err.response) message.error(err.response.data.Message);
    }
  };

  const columns = [
    { title: 'Tên Bậc', dataIndex: 'stepName', key: 'stepName', className: 'font-semibold' },
    { title: 'Mức Lương P1 (VNĐ)', dataIndex: 'p1Salary', key: 'p1Salary', render: (val: number) => val.toLocaleString('vi-VN') },
    { title: 'Ngày Áp Dụng', dataIndex: 'effectiveDate', key: 'effectiveDate', render: (val: string) => dayjs(val).format('DD/MM/YYYY') },
    { title: 'Hành Động', key: 'actions', align: 'right' as const, render: (_: unknown, record: SalaryStepDto) => (
      <Space>
        <Button size="small" type="dashed" icon={<HistoryOutlined />} onClick={() => viewHistory(record.stepName)}>Lịch sử</Button>
        <Button size="small" type="primary" ghost icon={<EditOutlined />} onClick={() => handleOpenUpdateVersion(record)}>Cập nhật mới</Button>
        <Button size="small" danger icon={<DeleteOutlined />} onClick={() => handleDelete(record.stepName)} />
      </Space>
    )}
  ];

  const historyColumns = [
    { title: 'Mức Lương P1 (VNĐ)', dataIndex: 'p1Salary', render: (val: number) => val.toLocaleString('vi-VN') },
    { title: 'Từ Ngày', dataIndex: 'effectiveDate', render: (val: string) => dayjs(val).format('DD/MM/YYYY') },
    { title: 'Đến Ngày', dataIndex: 'endDate', render: (val: string) => val ? dayjs(val).format('DD/MM/YYYY') : 'Hiện tại' },
    { title: 'Trạng Thái', dataIndex: 'status', render: (val: string) => val === 'HIEU_LUC' ? <Tag color="green">Hiệu lực</Tag> : <Tag color="default">Hết hạn</Tag> }
  ];

  return (
    <Drawer title={`Cấu Hình Bậc Lương - ${positionName}`} width={800} onClose={onClose} open={isOpen} destroyOnClose>
      <div className="mb-4 flex justify-between">
        <span className="text-gray-500 italic">Lưu ý: Không dùng xóa để cập nhật tiền. Hãy chọn "Cập nhật mới" để lưu lịch sử.</span>
        <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenCreate}>Thêm Bậc Mới</Button>
      </div>

      <Table dataSource={activeSteps} columns={columns} rowKey="id" loading={loading} pagination={false} />

      <Modal title={isUpdatingVersion ? "Cập Nhật Phiên Bản Lương" : "Thêm Mới Bậc Lương"} open={formModalOpen} onOk={handleFormSubmit} onCancel={() => setFormModalOpen(false)} destroyOnClose>
        <Form form={form} layout="vertical" className="mt-4">
          <Form.Item name="stepName" label="Tên Bậc" rules={[{ required: true }]}>
            <Input disabled={isUpdatingVersion} placeholder="VD: Bậc 1" />
          </Form.Item>
          <Form.Item name={isUpdatingVersion ? "newP1Salary" : "p1Salary"} label="Mức Lương P1 (VNĐ)" rules={[{ required: true }]}>
            <InputNumber className="w-full" formatter={value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')} parser={value => value!.replace(/\$\s?|(,*)/g, '')} />
          </Form.Item>
          <Form.Item name={isUpdatingVersion ? "newEffectiveDate" : "effectiveDate"} label="Ngày Áp Dụng Mới" rules={[{ required: true }]}>
            <DatePicker className="w-full" format="DD/MM/YYYY" />
          </Form.Item>
        </Form>
      </Modal>

      <Modal title={`Lịch Sử Thay Đổi`} open={historyModalOpen} onCancel={() => setHistoryModalOpen(false)} footer={null} width={600}>
        <Table dataSource={historyData} columns={historyColumns} rowKey="id" pagination={false} size="small" />
      </Modal>
    </Drawer>
  );
};