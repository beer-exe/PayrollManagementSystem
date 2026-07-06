import React, { useEffect } from 'react';
import { Card, Table, Button, InputNumber, Input, Form, Tag, Spin, Result } from 'antd';
import { useParams, useNavigate } from 'react-router-dom';
import { usePhieuDanhGia } from '../hooks/usePhieuDanhGia';

export const TuDanhGiaForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { detail, loading, fetchById, submitTuDanhGia } = usePhieuDanhGia();
  const [form] = Form.useForm();

  useEffect(() => {
    if (id) {
      fetchById(id);
    }
  }, [id, fetchById]);

  useEffect(() => {
    if (detail && detail.chiTietDanhGias) {
      const formValues: any = {};
      detail.chiTietDanhGias.forEach(c => {
        formValues[`diem_${c.idChiTiet}`] = c.diemTuDanhGia;
        formValues[`nhanXet_${c.idChiTiet}`] = c.nhanXetNhanVien;
      });
      form.setFieldsValue(formValues);
    }
  }, [detail, form]);

  const handleSave = async (isSubmit: boolean) => {
    const values = await form.validateFields();
    if (!detail) return;

    const chiTiets = detail.chiTietDanhGias.map(c => ({
      idChiTiet: c.idChiTiet,
      diemTuDanhGia: values[`diem_${c.idChiTiet}`] || 0,
      nhanXetNhanVien: values[`nhanXet_${c.idChiTiet}`] || ''
    }));

    const success = await submitTuDanhGia({
      idPhieu: detail.idPhieu,
      isSubmit,
      chiTiets
    });

    if (success) {
      navigate('/dashboard/danh-gia/tu-danh-gia');
    }
  };

  if (loading && !detail) return <Spin size="large" className="w-full mt-20 flex justify-center" />;
  if (!detail && !loading) return <Result status="404" title="Không tìm thấy phiếu" />;

  const isEditable = detail?.trangThai === 'CHO_NV_DANH_GIA';

  const columns = [
    { title: 'Tiêu chí', dataIndex: 'tenNangLuc', key: 'tenNangLuc', width: '20%' },
    { title: 'Yêu cầu tối thiểu', dataIndex: 'yeuCauToiThieu', key: 'yeuCauToiThieu', width: '30%' },
    { title: 'Tỷ trọng', dataIndex: 'tyTrong', key: 'tyTrong', align: 'center' as const },
    {
      title: 'Tự đánh giá',
      key: 'diemTuDanhGia',
      width: '15%',
      render: (_: unknown, record: any) => (
        <Form.Item 
          name={`diem_${record.idChiTiet}`} 
          style={{ marginBottom: 0 }}
          rules={[{ required: isEditable, message: 'Nhập điểm' }]}
        >
          <InputNumber min={0} disabled={!isEditable} className="w-full" />
        </Form.Item>
      )
    },
    {
      title: 'Nhận xét cá nhân',
      key: 'nhanXetNhanVien',
      width: '20%',
      render: (_: unknown, record: any) => (
        <Form.Item name={`nhanXet_${record.idChiTiet}`} style={{ marginBottom: 0 }}>
          <Input.TextArea rows={2} disabled={!isEditable} placeholder="Giải trình thêm..." />
        </Form.Item>
      )
    }
  ];

  return (
    <Card 
      title={<span className="text-xl font-bold">Phiếu tự đánh giá: {detail?.tenKyDanhGia}</span>}
      extra={
        <Tag color={isEditable ? 'blue' : 'green'} className="text-sm px-3 py-1">
          {detail?.trangThai}
        </Tag>
      }
    >
      <Form form={form} layout="vertical">
        <Table 
          columns={columns} 
          dataSource={detail?.chiTietDanhGias} 
          rowKey="idChiTiet" 
          pagination={false} 
          bordered
        />

        {isEditable && (
          <div className="flex justify-end mt-6 gap-4">
            <Button size="large" onClick={() => handleSave(false)}>Lưu nháp</Button>
            <Button size="large" type="primary" onClick={() => handleSave(true)}>Gửi Quản lý duyệt</Button>
          </div>
        )}
        
        {!isEditable && (
          <div className="mt-6 p-4 bg-gray-50 dark:bg-gray-800 border dark:border-gray-700 rounded-lg">
            <h3 className="font-bold mb-2">Kết quả đánh giá từ Quản lý:</h3>
            <p><strong>Điểm tổng hợp:</strong> {detail?.diemTongHop ?? 'Chưa chấm'}</p>
            <p><strong>Hệ số P2:</strong> {detail?.heSoP2 ?? 'Chưa chấm'}</p>
            <p><strong>Xếp loại:</strong> {detail?.xepLoai ?? 'Chưa xếp loại'}</p>
            <p><strong>Nhận xét chung:</strong> {detail?.nhanXetChung ?? 'Không có'}</p>
          </div>
        )}
      </Form>
    </Card>
  );
};
