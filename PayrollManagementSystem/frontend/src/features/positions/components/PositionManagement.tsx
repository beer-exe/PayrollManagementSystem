import React, { useEffect, useState } from "react";
import {
  Table,
  Input,
  Select,
  Button,
  Modal,
  Form,
  Tag,
  Space,
} from "antd";
import {
  PlusOutlined,
  SearchOutlined,
  EditOutlined,
  RetweetOutlined,
} from "@ant-design/icons";
import { usePositions } from "../hooks/usePositions";
import { PositionDto } from "../types/position.types";

export const PositionManagement: React.FC = () => {
  const {
    positions,
    loading,
    fetchPositions,
    createPosition,
    updatePosition,
    toggleStatus,
  } = usePositions();

  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<string | undefined>(
    undefined,
  );

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingPos, setEditingPos] = useState<PositionDto | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchPositions(searchTerm, statusFilter);
  }, [searchTerm, statusFilter, fetchPositions]);

  const handleOpenModal = (record?: PositionDto) => {
    setEditingPos(record || null);
    if (record) {
      form.setFieldsValue(record);
    } else {
      form.resetFields();
    }
    setIsModalOpen(true);
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      let success = false;
      if (editingPos) {
        success = await updatePosition(editingPos.idChucVu, {
          ...values,
          idChucVu: editingPos.idChucVu,
        });
      } else {
        success = await createPosition(values);
      }

      if (success) {
        setIsModalOpen(false);
        fetchPositions(searchTerm, statusFilter);
      }
    } catch (info) {
      console.log("Validate Failed:", info);
    }
  };

  const handleToggleStatus = (record: PositionDto) => {
    Modal.confirm({
      title: "Xác nhận chuyển trạng thái",
      content:
        record.trangThai === "HOAT_DONG"
          ? `Bạn có chắc muốn vô hiệu hóa chức vụ "${record.tenChucVu}"?`
          : `Bạn có muốn kích hoạt lại chức vụ "${record.tenChucVu}"?`,
      okText: "Đồng ý",
      cancelText: "Hủy",
      onOk: async () => {
        const success = await toggleStatus(record.idChucVu);
        if (success) fetchPositions(searchTerm, statusFilter);
      },
    });
  };

  const columns = [
    {
      title: "Mã Chức Vụ",
      dataIndex: "idChucVu",
      key: "idChucVu",
      width: 150,
      render: (text: string) => <span className="font-mono">{text}</span>,
    },
    {
      title: "Tên Chức Vụ",
      dataIndex: "tenChucVu",
      key: "tenChucVu",
      render: (text: string) => (
        <span className="font-semibold text-gray-800 dark:text-white">
          {text}
        </span>
      ),
    },
    {
      title: "Mô Tả Công Việc",
      dataIndex: "moTaCongViec",
      key: "moTaCongViec",
      ellipsis: true,
    },
    {
      title: "Trạng Thái",
      dataIndex: "trangThai",
      key: "trangThai",
      width: 150,
      align: "center" as const,
      render: (status: string) =>
        status === "HOAT_DONG" ? (
          <Tag color="success" className="rounded-full px-3">
            Đang hoạt động
          </Tag>
        ) : (
          <Tag color="default" className="rounded-full px-3 text-gray-500">
            Ngừng HĐ
          </Tag>
        ),
    },
    {
      title: "Hành Động",
      key: "actions",
      width: 300,
      align: "right" as const,
      render: (_: unknown, record: PositionDto) => (
        <Space size="small">

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
            icon={<RetweetOutlined />}
            className={
              record.trangThai === "HOAT_DONG"
                ? "text-amber-600 hover:bg-amber-50"
                : "text-emerald-600 hover:bg-emerald-50"
            }
            onClick={() => handleToggleStatus(record)}
          >
            {record.trangThai === "HOAT_DONG" ? "Vô hiệu hóa" : "Kích hoạt"}
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div className="p-4 sm:p-6 h-full flex flex-col bg-gray-50/50 dark:bg-gray-900 min-w-0 overflow-hidden">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6 gap-4">
        <div>
          <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
            Danh Mục Chức Vụ
          </h2>
          <p className="text-sm text-gray-500 mt-1">
            Quản lý các chức vụ và mô tả công việc (3P)
          </p>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => handleOpenModal()}
          className="bg-violet-600 hover:bg-violet-700 h-10 px-5 rounded-lg shadow-sm"
        >
          Thêm Chức Vụ
        </Button>
      </div>

      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-100 flex flex-col flex-1 overflow-hidden min-h-0">
        <div className="flex flex-col sm:flex-row p-4 border-b border-gray-100 gap-4 flex-shrink-0">
          <Input
            placeholder="Tìm theo Mã, Tên chức vụ..."
            prefix={<SearchOutlined className="text-gray-400" />}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            allowClear
            className="w-full sm:max-w-md h-10 rounded-lg"
          />
          <Select
            placeholder="Lọc theo trạng thái"
            allowClear
            value={statusFilter}
            onChange={setStatusFilter}
            className="w-full sm:w-48 h-10"
            options={[
              { value: "HOAT_DONG", label: "Đang hoạt động" },
              { value: "NGUNG_HOAT_DONG", label: "Ngừng hoạt động" },
            ]}
          />
        </div>

        <div className="grid grid-cols-1 w-full flex-1 min-h-0">
          <Table
            dataSource={positions}
            columns={columns}
            rowKey="idChucVu"
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
            {editingPos ? "Cập Nhật Chức Vụ" : "Thêm Mới Chức Vụ"}
          </h3>
        }
        open={isModalOpen}
        onOk={handleSubmit}
        onCancel={() => setIsModalOpen(false)}
        okText="Lưu lại"
        cancelText="Hủy bỏ"
        destroyOnClose
        okButtonProps={{
          className: "bg-violet-600 hover:bg-violet-700 rounded-lg",
        }}
        cancelButtonProps={{ className: "rounded-lg" }}
      >
        <Form form={form} layout="vertical" className="mt-2">
          <Form.Item
            name="idChucVu"
            label={
              <span className="font-semibold text-gray-700">Mã Chức Vụ</span>
            }
            rules={[{ required: true, message: "Vui lòng nhập mã chức vụ!" }]}
          >
            <Input
              placeholder="VD: CV_TRUONGPHONG"
              size="large"
              disabled={!!editingPos}
              className="rounded-lg"
            />
          </Form.Item>
          <Form.Item
            name="tenChucVu"
            label={
              <span className="font-semibold text-gray-700">Tên Chức Vụ</span>
            }
            rules={[{ required: true, message: "Vui lòng nhập tên chức vụ!" }]}
          >
            <Input
              placeholder="VD: Trưởng Phòng"
              size="large"
              className="rounded-lg"
            />
          </Form.Item>
          <Form.Item
            name="moTaCongViec"
            label={
              <span className="font-semibold text-gray-700">
                Mô Tả Công Việc
              </span>
            }
          >
            <Input.TextArea
              placeholder="Mô tả sơ lược về công việc, trách nhiệm..."
              rows={4}
              className="rounded-lg"
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};
