import React, { useEffect, useState } from 'react';
import { Card, Form, InputNumber, Input, Button, Table, Typography, Space, Tag, Spin } from 'antd';
import { useParams, useNavigate } from 'react-router-dom';
import { usePhieuDanhGia } from '../hooks/usePhieuDanhGia';
import { useMucQuyDoi } from '../hooks/useMucQuyDoi';

const { Title, Text } = Typography;

export const DuyetDanhGiaForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { detail, loading, fetchById, submitManagerEvaluation } = usePhieuDanhGia();
  const { data: rules, fetchQuyDoi } = useMucQuyDoi();
  const [form] = Form.useForm();
  const [submitting, setSubmitting] = useState(false);
  const chiTietsForm = Form.useWatch('chiTiets', form);

  useEffect(() => {
    fetchQuyDoi();
  }, [fetchQuyDoi]);

  useEffect(() => {
    if (id) {
      fetchById(id);
    }
  }, [id, fetchById]);

  useEffect(() => {
    if (detail) {
      const chiTiets = detail.chiTietDanhGias.map(item => ({
        idChiTiet: item.idChiTiet,
        diemQuanLyDanhGia: item.diemQuanLyDanhGia || item.diemTuDanhGia || 0,
        nhanXetQuanLy: item.nhanXetQuanLy || ''
      }));
      
      form.setFieldsValue({
        nhanXetChung: detail.nhanXetChung || '',
        chiTiets
      });
    }
  }, [detail, form]);

  const onFinish = async (values: any, isSubmit: boolean) => {
    if (!id) return;
    setSubmitting(true);
    const success = await submitManagerEvaluation({
      idPhieu: id,
      isSubmit,
      nhanXetChung: values.nhanXetChung,
      chiTiets: values.chiTiets
    });
    setSubmitting(false);

    if (success) {
      navigate('/dashboard/danh-gia/duyet-danh-gia');
    }
  };

  if (loading || !detail) return <Spin className="flex justify-center mt-20" size="large" />;

  const isEditable = detail.canEvaluate && detail.trangThai === 'CHO_QL_DANH_GIA';
  const isCompleted = detail.trangThai === 'DA_HOAN_THANH';

  // Tính điểm Nhân viên
  const empScore = detail.chiTietDanhGias.reduce((sum, item) => sum + (item.diemTuDanhGia || 0) * item.tyTrong, 0);

  // Tính điểm Quản lý (Live)
  let mgrScore = 0;
  if (chiTietsForm && Array.isArray(chiTietsForm)) {
    mgrScore = chiTietsForm.reduce((sum, formItem, index) => {
      const w = detail.chiTietDanhGias[index]?.tyTrong || 0;
      return sum + (formItem?.diemQuanLyDanhGia || 0) * w;
    }, 0);
  } else if (detail) {
    mgrScore = detail.chiTietDanhGias.reduce((sum, item) => sum + (item.diemQuanLyDanhGia || item.diemTuDanhGia || 0) * item.tyTrong, 0);
  }

  // Tra cứu bảng quy đổi
  const getQuyDoi = (score: number) => {
    const rule = rules.find(r => score >= r.diemToiThieu && score <= r.diemToiDa);
    if (!rule) return { xepLoai: 'Chưa xác định', heSo: 0 };
    return { xepLoai: rule.xepLoai, heSo: rule.heSoP2 };
  };

  const empResult = getQuyDoi(empScore);
  const mgrResult = getQuyDoi(mgrScore);

  const columns = [
    { 
      title: 'Tiêu chí Năng lực', 
      dataIndex: 'tenNangLuc', 
      key: 'tenNangLuc',
      width: '20%',
      render: (text: string, record: any) => (
        <div>
          <div className="font-bold">{text}</div>
          <div className="text-gray-500 text-xs mt-1">Trọng số: {record.tyTrong}</div>
        </div>
      )
    },
    { title: 'Yêu cầu tối thiểu', dataIndex: 'yeuCauToiThieu', key: 'yeuCauToiThieu', width: '25%' },
    { 
      title: 'NV Tự đánh giá', 
      key: 'tuDanhGia',
      width: '20%',
      render: (_: unknown, record: any) => (
        <div className="bg-gray-50 p-2 rounded border">
          <div className="font-bold text-blue-600">Điểm: {record.diemTuDanhGia}</div>
          <div className="text-sm mt-1">{record.nhanXetNhanVien || <span className="italic text-gray-400">Không có nhận xét</span>}</div>
        </div>
      )
    },
    {
      title: 'Quản lý Đánh giá',
      key: 'quanLyDanhGia',
      width: '25%',
      render: (_: unknown, __: unknown, index: number) => {
        return (
          <div className="bg-blue-50 p-2 rounded border border-blue-200">
            <Form.Item name={['chiTiets', index, 'idChiTiet']} hidden><Input /></Form.Item>
            
            <Form.Item 
              name={['chiTiets', index, 'diemQuanLyDanhGia']} 
              label="Điểm"
              rules={[{ required: true, message: 'Nhập điểm' }]}
              className="mb-2"
            >
              <InputNumber 
                className="w-full" 
                min={0} 
                disabled={!isEditable} 
              />
            </Form.Item>
            <Form.Item 
              name={['chiTiets', index, 'nhanXetQuanLy']} 
              className="mb-0"
            >
              <Input.TextArea 
                placeholder="Nhận xét của QL..." 
                rows={2} 
                disabled={!isEditable} 
              />
            </Form.Item>
          </div>
        );
      }
    }
  ];

  return (
    <Card 
      title={<Title level={4}>Duyệt Phiếu Đánh Giá</Title>}
      extra={<Tag color={isCompleted ? "green" : "blue"}>{detail.trangThai}</Tag>}
    >
      <div className="mb-6 bg-white dark:bg-gray-800 p-4 rounded-lg border dark:border-gray-700">
        <Space size="large" wrap>
          <Text><strong>Kỳ đánh giá:</strong> {detail.tenKyDanhGia}</Text>
          <Text><strong>CCCD Nhân viên:</strong> {detail.cccdNhanVien}</Text>
        </Space>
      </div>

      <div className="mb-6 grid grid-cols-2 gap-4">
        <Card title="Nhân viên Tự đánh giá" size="small" className="bg-gray-50">
          <div className="flex justify-between items-center text-lg">
            <span>Tổng điểm: <strong>{empScore.toFixed(2)}</strong></span>
            <span>Hệ số P2: <strong className="text-green-600">{empResult.heSo}</strong></span>
            <span>Xếp loại: <Tag color="blue">{empResult.xepLoai}</Tag></span>
          </div>
        </Card>
        
        <Card title="Quản lý Đánh giá (Dự kiến)" size="small" className="bg-blue-50 border-blue-200">
          <div className="flex justify-between items-center text-lg">
            <span>Tổng điểm: <strong>{mgrScore.toFixed(2)}</strong></span>
            <span>Hệ số P2: <strong className="text-green-600">{mgrResult.heSo}</strong></span>
            <span>Xếp loại: <Tag color="blue">{mgrResult.xepLoai}</Tag></span>
          </div>
        </Card>
      </div>

      <Form form={form} layout="vertical">
        <Table 
          columns={columns} 
          dataSource={detail.chiTietDanhGias} 
          rowKey="idChiTiet" 
          pagination={false}
          bordered
        />

        <div className="mt-6">
          <Form.Item name="nhanXetChung" label={<span className="font-bold text-lg">Nhận xét chung của Quản lý</span>}>
            <Input.TextArea rows={4} disabled={!isEditable} placeholder="Đánh giá tổng quan về nhân viên..." />
          </Form.Item>
        </div>

        <div className="flex justify-end gap-4 mt-6">
          <Button 
            onClick={() => navigate('/dashboard/danh-gia/duyet-danh-gia')}
          >
            Quay lại
          </Button>
          {isEditable && (
            <>
              <Button 
                onClick={() => form.validateFields().then(v => onFinish(v, false))}
                loading={submitting}
              >
                Lưu nháp
              </Button>
              <Button 
                type="primary" 
                onClick={() => form.validateFields().then(v => onFinish(v, true))}
                loading={submitting}
              >
                Chốt đánh giá
              </Button>
            </>
          )}
        </div>
      </Form>
    </Card>
  );
};
