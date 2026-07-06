import React, { useEffect, useState } from 'react';
import { Card, Table, Button, Modal, Form, InputNumber, Input, Select, Space, Popconfirm } from 'antd';
import { useKhungNangLuc } from '../hooks/useKhungNangLuc';
import { positionApi } from '@/features/positions/api/positionApi';
import { PositionDto } from '@/features/positions/types/position.types';

export const KhungNangLucManagement: React.FC = () => {
  const [positions, setPositions] = useState<PositionDto[]>([]);
  const [selectedChucVu, setSelectedChucVu] = useState<string | undefined>(undefined);
  
  const { data, loading, fetchByChucVu, createCriteria, updateCriteria, deleteCriteria } = useKhungNangLuc();
  
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchPositions();
  }, []);

  useEffect(() => {
    if (selectedChucVu) {
      fetchByChucVu(selectedChucVu);
    }
  }, [selectedChucVu, fetchByChucVu]);

  const fetchPositions = async () => {
    try {
      const res = await positionApi.getPositions();
      if (res.succeeded) {
        setPositions(res.data);
      }
    } catch (e) {
      console.error(e);
    }
  };

  const handleAdd = () => {
    setEditingId(null);
    form.resetFields();
    setIsModalVisible(true);
  };

  const handleEdit = (record: any) => {
    setEditingId(record.idTieuChi);
    form.setFieldsValue({
      tenNangLuc: record.tenNangLuc,
      yeuCauToiThieu: record.yeuCauToiThieu,
      tyTrong: record.tyTrong
    });
    setIsModalVisible(true);
  };

  const handleDelete = async (id: string) => {
    const success = await deleteCriteria(id);
    if (success && selectedChucVu) {
      fetchByChucVu(selectedChucVu);
    }
  };

  const handleSubmit = async (values: any) => {
    if (!selectedChucVu) return;

    let success = false;
    if (editingId) {
      success = await updateCriteria(editingId, {
        idTieuChi: editingId,
        ...values
      });
    } else {
      success = await createCriteria({
        idChucVu: selectedChucVu,
        ...values
      });
    }

    if (success) {
      setIsModalVisible(false);
      fetchByChucVu(selectedChucVu);
    }
  };

  const columns = [
    { title: 'Tên năng lực', dataIndex: 'tenNangLuc', key: 'tenNangLuc', width: '25%' },
    { title: 'Yêu cầu tối thiểu', dataIndex: 'yeuCauToiThieu', key: 'yeuCauToiThieu', width: '35%' },
    { title: 'Tỷ trọng', dataIndex: 'tyTrong', key: 'tyTrong', align: 'center' as const },
    {
      title: 'Hành động',
      key: 'action',
      render: (_: unknown, record: any) => (
        <Space>
          <Button type="link" onClick={() => handleEdit(record)}>Sửa</Button>
          <Popconfirm title="Xóa tiêu chí này?" onConfirm={() => handleDelete(record.idTieuChi)}>
            <Button type="link" danger>Xóa</Button>
          </Popconfirm>
        </Space>
      )
    }
  ];

  return (
    <Card 
      title="Cấu hình Khung Năng Lực (P2)" 
      extra={
        <Button 
          type="primary" 
          disabled={!selectedChucVu} 
          onClick={handleAdd}
        >
          Thêm tiêu chí
        </Button>
      }
    >
      <div className="mb-4">
        <label className="font-bold mr-4">Chọn Chức vụ:</label>
        <Select
          showSearch
          className="w-64"
          placeholder="Chọn chức vụ cần cấu hình"
          options={positions.map(p => ({ label: p.tenChucVu, value: p.idChucVu }))}
          onChange={setSelectedChucVu}
          value={selectedChucVu}
          filterOption={(input, option) => 
            (option?.label ?? '').toString().toLowerCase().includes(input.toLowerCase())
          }
        />
      </div>

      {!selectedChucVu ? (
        <div className="text-center py-10 text-gray-500 bg-gray-50 dark:bg-gray-800 rounded">
          Vui lòng chọn một chức vụ để xem và cấu hình khung năng lực.
        </div>
      ) : (
        <Table 
          columns={columns} 
          dataSource={data} 
          rowKey="idTieuChi" 
          loading={loading}
          pagination={false}
          bordered
        />
      )}

      <Modal
        title={editingId ? "Sửa Tiêu chí Năng lực" : "Thêm Tiêu chí Năng lực"}
        open={isModalVisible}
        onCancel={() => setIsModalVisible(false)}
        onOk={() => form.submit()}
        width={600}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item name="tenNangLuc" label="Tên năng lực / Tiêu chí" rules={[{ required: true, message: 'Vui lòng nhập tên năng lực' }]}>
            <Input placeholder="Vd: Kỹ năng giải quyết vấn đề" />
          </Form.Item>
          <Form.Item name="yeuCauToiThieu" label="Yêu cầu tối thiểu" rules={[{ required: true }]}>
            <Input.TextArea rows={3} placeholder="Mô tả các yêu cầu..." />
          </Form.Item>
          <div className="flex gap-4">
            <Form.Item className="flex-1" name="tyTrong" label="Tỷ trọng" rules={[{ required: true }]}>
              <InputNumber className="w-full" step={0.1} min={0} />
            </Form.Item>
          </div>
        </Form>
      </Modal>
    </Card>
  );
};
