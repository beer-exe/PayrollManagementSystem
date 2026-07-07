import React, { useEffect, useState } from 'react';
import { Card, Table, Button, Modal, Form, InputNumber, Input, Select, message, Space, Popconfirm } from 'antd';
import { PlusOutlined, MinusCircleOutlined } from '@ant-design/icons';
import { useKhungNangLuc } from '../hooks/useKhungNangLuc';
import { positionApi } from '@/features/positions/api/positionApi';
import { PositionDto } from '@/features/positions/types/position.types';

// Array of vibrant colors for the donut chart slices
const CHART_COLORS = [
  '#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', 
  '#ec4899', '#06b6d4', '#84cc16', '#f97316', '#6366f1'
];

export const KhungNangLucManagement: React.FC = () => {
  const [positions, setPositions] = useState<PositionDto[]>([]);
  const [selectedChucVu, setSelectedChucVu] = useState<string | undefined>(undefined);
  
  const { data, loading, fetchByChucVu, createCriteria, updateCriteria, deleteCriteria } = useKhungNangLuc();
  
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [form] = Form.useForm();
  
  // Watch the criteria list to compute the total weight in real-time
  const criteriaList = Form.useWatch('criteria', form) || [];
  const totalWeightPercent = criteriaList.reduce((sum: number, item: any) => sum + (Number(item?.tyTrong) || 0), 0);
  const isOverweight = totalWeightPercent > 100;

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

  const handleOpenConfig = () => {
    // Map existing data to form, converting tyTrong from 0-1 to 0-100
    const initialValues = data.map(item => ({
      idTieuChi: item.idTieuChi,
      tenNangLuc: item.tenNangLuc,
      moTa: item.moTa,
      tyTrong: Number((item.tyTrong * 100).toFixed(1))
    }));
    
    form.setFieldsValue({ criteria: initialValues });
    setIsModalVisible(true);
  };

  const handleSubmit = async (values: any) => {
    if (!selectedChucVu) return;
    
    if (totalWeightPercent > 100) {
      message.error("Tổng tỷ trọng không được vượt quá 100%");
      return;
    }

    const currentCriteria = values.criteria || [];
    
    // Find what to create, update, delete
    const existingIdsInForm = currentCriteria.map((c: any) => c.idTieuChi).filter(Boolean);
    const deletedItems = data.filter(d => !existingIdsInForm.includes(d.idTieuChi));
    
    try {
      // 1. Delete removed items
      const deletePromises = deletedItems.map(d => deleteCriteria(d.idTieuChi));
      
      // 2. Update existing items
      const updatePromises = currentCriteria
        .filter((c: any) => c.idTieuChi)
        .map((c: any) => updateCriteria(c.idTieuChi, {
          idTieuChi: c.idTieuChi,
          tenNangLuc: c.tenNangLuc,
          moTa: c.moTa,
          tyTrong: c.tyTrong / 100
        }));
        
      // 3. Create new items
      const createPromises = currentCriteria
        .filter((c: any) => !c.idTieuChi)
        .map((c: any) => createCriteria({
          idChucVu: selectedChucVu,
          tenNangLuc: c.tenNangLuc,
          moTa: c.moTa,
          tyTrong: c.tyTrong / 100
        }));

      await Promise.all([...deletePromises, ...updatePromises, ...createPromises]);
      
      message.success("Lưu cấu hình tiêu chí thành công");
      setIsModalVisible(false);
      fetchByChucVu(selectedChucVu);
    } catch (e) {
      message.error("Có lỗi xảy ra khi lưu cấu hình");
    }
  };

  // Generate CSS background for conic-gradient pie chart
  let cumulativePercent = 0;
  const gradientStops = criteriaList.map((item: any, index: number) => {
    const p = Number(item?.tyTrong) || 0;
    if (p <= 0) return null;
    const start = cumulativePercent;
    cumulativePercent += p;
    const color = CHART_COLORS[index % CHART_COLORS.length];
    return `${color} ${start}%, ${color} ${cumulativePercent}%`;
  }).filter(Boolean);
  
  // If there's remaining space, fill it with gray
  if (cumulativePercent < 100) {
    gradientStops.push(`#e5e7eb ${cumulativePercent}%, #e5e7eb 100%`);
  }

  const conicGradient = gradientStops.length > 0 
    ? `conic-gradient(${gradientStops.join(', ')})`
    : 'conic-gradient(#e5e7eb 0 100%)';

  const columns = [
    { title: 'Tên năng lực', dataIndex: 'tenNangLuc', key: 'tenNangLuc', width: '35%' },
    { title: 'Mô tả', dataIndex: 'moTa', key: 'moTa', width: '40%' },
    { title: 'Tỷ trọng', dataIndex: 'tyTrong', key: 'tyTrong', align: 'center' as const, render: (val: number) => `${Number((val * 100).toFixed(1))}%` },
    {
      title: 'Hành động',
      key: 'action',
      width: '15%',
      align: 'right' as const,
      render: (_: unknown, record: any) => (
        <Space>
          <Button type="link" onClick={handleOpenConfig}>Sửa</Button>
          <Popconfirm title="Xóa tiêu chí này?" onConfirm={async () => {
            const success = await deleteCriteria(record.idTieuChi);
            if (success && selectedChucVu) fetchByChucVu(selectedChucVu);
          }}>
            <Button type="link" danger>Xóa</Button>
          </Popconfirm>
        </Space>
      )
    }
  ];

  return (
    <Card 
      title={
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3 whitespace-normal">
          <span>Cấu hình Khung Năng Lực (P2)</span>
          <Button 
            type="primary" 
            disabled={!selectedChucVu} 
            onClick={handleOpenConfig}
          >
            Cấu hình tiêu chí
          </Button>
        </div>
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
          scroll={{ x: 'max-content' }}
          bordered
        />
      )}

      <Modal
        title={<span className="text-xl font-bold">Cấu Hình Tiêu Chí Năng Lực</span>}
        open={isModalVisible}
        onCancel={() => setIsModalVisible(false)}
        onOk={() => form.submit()}
        okButtonProps={{ disabled: isOverweight }}
        width={1000}
        destroyOnClose
        style={{ top: 20 }}
        styles={{ body: { maxHeight: 'calc(100vh - 150px)', overflowY: 'auto', overflowX: 'hidden' } }}
      >
        <div className="flex flex-col lg:flex-row gap-6 mt-4">
          
          {/* Left side: Dynamic Form List */}
          <div className="flex-1 bg-gray-50 dark:bg-gray-800/50 p-4 rounded-xl border border-gray-100">
            <Form form={form} layout="vertical" onFinish={handleSubmit}>
              <Form.List name="criteria">
                {(fields, { add, remove }) => (
                  <>
                    {fields.map(({ key, name, ...restField }, index) => (
                      <div key={key} className="flex gap-4 items-start mb-4 bg-white p-4 rounded-lg shadow-sm border border-gray-100 relative group">
                        <div className="w-1 h-full absolute left-0 top-0 rounded-l-lg" style={{ backgroundColor: CHART_COLORS[index % CHART_COLORS.length] }}></div>
                        <div className="flex-1">
                          <Form.Item
                            {...restField}
                            name={[name, 'idTieuChi']}
                            hidden
                          ><Input /></Form.Item>

                          <div className="flex flex-col sm:flex-row gap-0 sm:gap-4">
                            <Form.Item
                              {...restField}
                              name={[name, 'tenNangLuc']}
                              label="Tên tiêu chí"
                              className="flex-1 mb-2"
                              rules={[{ required: true, message: 'Nhập tên' }]}
                            >
                              <Input placeholder="Kỹ năng giải quyết vấn đề" />
                            </Form.Item>
                            
                            <Form.Item
                              {...restField}
                              name={[name, 'tyTrong']}
                              label="Tỷ trọng (%)"
                              className="w-full sm:w-32 mb-2"
                              rules={[{ required: true, message: 'Nhập %' }]}
                            >
                              <InputNumber min={0.1} max={100} step={1} className="w-full" addonAfter="%" />
                            </Form.Item>
                          </div>

                          <Form.Item
                            {...restField}
                            name={[name, 'moTa']}
                            label="Mô tả"
                            className="mb-0"
                          >
                            <Input.TextArea rows={2} placeholder="Nhập mô tả chi tiết cho tiêu chí này (không bắt buộc)..." />
                          </Form.Item>
                        </div>
                        
                        <Button 
                          type="text" 
                          danger 
                          icon={<MinusCircleOutlined />} 
                          onClick={() => remove(name)}
                          className="mt-8 opacity-50 hover:opacity-100"
                        />
                      </div>
                    ))}
                    
                    <Button 
                      type="dashed" 
                      onClick={() => add()} 
                      block 
                      icon={<PlusOutlined />}
                      className="h-12 border-dashed border-gray-300 hover:border-violet-500 hover:text-violet-600"
                    >
                      Thêm tiêu chí mới
                    </Button>
                  </>
                )}
              </Form.List>
            </Form>
          </div>

          {/* Right side: Donut Chart & Stats */}
          <div className="w-full lg:w-72 flex flex-col items-center">
            <h3 className="font-semibold text-gray-700 mb-6">Phân Bổ Tỷ Trọng</h3>
            
            <div className="relative w-48 h-48 rounded-full shadow-inner flex items-center justify-center transition-all duration-300"
                 style={{ background: conicGradient }}>
              {/* Inner circle to make it a donut */}
              <div className="absolute w-32 h-32 bg-white dark:bg-gray-800 rounded-full flex flex-col items-center justify-center shadow-sm">
                <span className={`text-2xl font-bold ${isOverweight ? 'text-red-500' : 'text-gray-800 dark:text-gray-100'}`}>
                  {totalWeightPercent}%
                </span>
                <span className="text-xs text-gray-500 uppercase tracking-wider">Tổng cộng</span>
              </div>
            </div>

            <div className="mt-8 w-full space-y-2">
              <div className="flex justify-between text-sm">
                <span className="text-gray-500">Đã phân bổ:</span>
                <span className={`font-semibold ${isOverweight ? 'text-red-500' : 'text-gray-800'}`}>{totalWeightPercent}%</span>
              </div>
              <div className="flex justify-between text-sm">
                <span className="text-gray-500">Còn lại:</span>
                <span className="font-semibold text-gray-800">{Math.max(0, 100 - totalWeightPercent)}%</span>
              </div>
            </div>

            {isOverweight && (
              <div className="mt-4 p-3 bg-red-50 border border-red-100 text-red-600 text-sm rounded-lg text-center">
                Tổng tỷ trọng đang vượt quá 100%. Vui lòng điều chỉnh lại.
              </div>
            )}
          </div>

        </div>
      </Modal>
    </Card>
  );
};
