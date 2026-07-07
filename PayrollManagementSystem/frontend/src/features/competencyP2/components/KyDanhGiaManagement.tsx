import React, { useEffect, useState } from 'react';
import { Card, Table, Button, Modal, Form, Input, DatePicker, Tag, message, Popconfirm, Space } from 'antd';
import { useKyDanhGia } from '../hooks/useKyDanhGia';
import { kyDanhGiaApi } from '../api/kyDanhGiaApi';

export const KyDanhGiaManagement: React.FC = () => {
  const { data, loading, fetchKyDanhGia, createKyDanhGia } = useKyDanhGia();
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchKyDanhGia();
  }, [fetchKyDanhGia]);

  const handleAdd = async (values: any) => {
    const payload = {
      ...values,
      ngayBatDau: values.ngayBatDau.format('YYYY-MM-DD'),
      ngayKetThuc: values.ngayKetThuc.format('YYYY-MM-DD'),
    };
    const success = await createKyDanhGia(payload);
    if (success) {
      setIsModalVisible(false);
      form.resetFields();
      fetchKyDanhGia();
    }
  };

  const handleDelete = async (id: string) => {
    try {
      const res = await kyDanhGiaApi.delete(id);
      if (res.succeeded) {
        message.success('Xóa thành công');
        fetchKyDanhGia();
      }
    } catch (e: any) {
      message.error(e.response?.data?.Message || 'Xóa thất bại');
    }
  };

  const handleChangeStatus = async (id: string, status: number, force: boolean = false) => {
    try {
      const res = await kyDanhGiaApi.changeStatus(id, status, force);
      if (res.succeeded) {
        message.success('Cập nhật trạng thái thành công');
        fetchKyDanhGia();
      }
    } catch (e: any) {
      const errorMsg = e.response?.data?.Message;
      if (errorMsg === "HienTaiCoPhieuChuaXong") {
        Modal.confirm({
          title: 'Cảnh báo',
          content: 'Hiện tại có phiếu đánh giá chưa hoàn thành. Bạn có chắc chắn muốn ép chốt kỳ đánh giá này không? (Hệ số P2 sẽ chỉ được cập nhật cho các phiếu đã hoàn thành).',
          okText: 'Ép chốt',
          cancelText: 'Hủy',
          onOk: () => handleChangeStatus(id, status, true)
        });
      } else {
        message.error(errorMsg || 'Cập nhật thất bại');
      }
    }
  };

  const columns = [
    { title: 'Tên kỳ đánh giá', dataIndex: 'tenKyDanhGia', key: 'tenKyDanhGia' },
    { title: 'Ngày bắt đầu', dataIndex: 'ngayBatDau', key: 'ngayBatDau' },
    { title: 'Ngày kết thúc', dataIndex: 'ngayKetThuc', key: 'ngayKetThuc' },
    {
      title: 'Trạng thái',
      dataIndex: 'trangThai',
      key: 'trangThai',
      render: (status: string, record: any) => {
        let color = 'default';
        if (status === 'DANG_DANH_GIA') color = 'green';
        if (status === 'KHOI_TAO') color = 'blue';
        if (status === 'DA_CHOT') color = 'orange';
        if (status === 'DA_HUY') color = 'red';
        return <Tag color={color}>{record.tenTrangThai || status}</Tag>;
      }
    },
    {
      title: 'Hành động',
      key: 'action',
      render: (_: unknown, record: any) => (
        <Space>
          {record.trangThai === 'KHOI_TAO' && (
            <>
              <Popconfirm title="Mở kỳ đánh giá này? Nhân viên sẽ bắt đầu có thể tự đánh giá." onConfirm={() => handleChangeStatus(record.idKyDanhGia, 1)}>
                <Button type="link" style={{ color: 'green' }}>Mở đánh giá</Button>
              </Popconfirm>
              <Popconfirm title="Xóa kỳ đánh giá này?" onConfirm={() => handleDelete(record.idKyDanhGia)}>
                <Button type="link" danger>Xóa</Button>
              </Popconfirm>
            </>
          )}
          {record.trangThai === 'DANG_DANH_GIA' && (
            <>
              <Popconfirm title="Bạn có chắc muốn chốt kỳ đánh giá? Sau khi chốt, hệ số P2 sẽ được tự động cập nhật vào thông tin nhân sự." onConfirm={() => handleChangeStatus(record.idKyDanhGia, 2)}>
                <Button type="link" style={{ color: 'orange' }}>Chốt kỳ</Button>
              </Popconfirm>
              <Popconfirm title="Hủy kỳ đánh giá này? Tất cả phiếu sẽ bị hủy." onConfirm={() => handleChangeStatus(record.idKyDanhGia, 3)}>
                <Button type="link" danger>Hủy kỳ</Button>
              </Popconfirm>
            </>
          )}
        </Space>
      )
    }
  ];

  return (
    <Card 
      title={
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3 whitespace-normal">
          <span>Quản lý Kỳ đánh giá Năng lực</span>
          <Button type="primary" onClick={() => setIsModalVisible(true)}>
            Tạo kỳ đánh giá
          </Button>
        </div>
      }
    >
      <Table 
        columns={columns} 
        dataSource={data} 
        rowKey="idKyDanhGia" 
        loading={loading}
        scroll={{ x: 'max-content' }}
      />
      <Modal
        title="Tạo Kỳ đánh giá mới"
        open={isModalVisible}
        onCancel={() => setIsModalVisible(false)}
        onOk={() => form.submit()}
      >
        <Form form={form} layout="vertical" onFinish={handleAdd}>
          <Form.Item name="tenKyDanhGia" label="Tên kỳ đánh giá" rules={[{ required: true }]}>
            <Input placeholder="VD: Đánh giá năng lực cuối năm 2026" />
          </Form.Item>
          <Form.Item name="ngayBatDau" label="Ngày bắt đầu" rules={[{ required: true }]}>
            <DatePicker className="w-full" format="DD/MM/YYYY" />
          </Form.Item>
          <Form.Item name="ngayKetThuc" label="Ngày kết thúc" rules={[{ required: true }]}>
            <DatePicker className="w-full" format="DD/MM/YYYY" />
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
};
