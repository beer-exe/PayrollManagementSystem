import React, { useEffect, useState } from "react";
import { Modal, Form, Select, DatePicker, Input, message, Alert } from "antd";
import { departmentApi } from "../../api/departmentApi";
import { EmployeeInDepartmentDto } from "../../types/department.types";
import { PositionDto } from "@/features/positions/types/position.types";
import { salaryStepApi } from "@/features/salarySteps/api/salaryStepApi";
import { SalaryStepDto } from "@/features/salarySteps/types/salaryStep.types";

interface ChangePositionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  employee: EmployeeInDepartmentDto | null;
  positions: PositionDto[];
}

export const ChangePositionModal: React.FC<ChangePositionModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
  employee,
  positions,
}) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const [salarySteps, setSalarySteps] = useState<SalaryStepDto[]>([]);
  const [loadingSteps, setLoadingSteps] = useState(false);

  useEffect(() => {
    if (isOpen) {
      form.resetFields();
      setSalarySteps([]);
    }
  }, [isOpen, form]);

  const handlePositionChange = async (positionId: string) => {
    form.setFieldsValue({ idBacLuongMoi: undefined });
    if (!positionId) {
      setSalarySteps([]);
      return;
    }

    setLoadingSteps(true);
    try {
      const res = await salaryStepApi.getActive(positionId);
      if (res.succeeded) {
        setSalarySteps(res.data);
      }
    } catch (error) {
      message.error("Lỗi tải danh sách bậc lương cho chức vụ này.");
    } finally {
      setLoadingSteps(false);
    }
  };

  const handleSubmit = async () => {
    if (!employee) return;

    try {
      const values = await form.validateFields();
      setLoading(true);

      const payload = {
        soQuyetDinh: values.soQuyetDinh,
        cccd: employee.cccd,
        idChucVuMoi: values.idChucVuMoi,
        idBacLuongMoi: values.idBacLuongMoi,
        ngayHieuLuc: values.ngayHieuLuc.format("YYYY-MM-DD"),
        lyDo: values.lyDo,
      };

      const res = await departmentApi.changePosition(payload);
      if (res.succeeded) {
        message.success("Thay đổi chức vụ thành công!");
        onSuccess();
        onClose();
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      message.error(
        err.response?.data?.Message ||
          "Lỗi khi lưu quyết định thay đổi chức vụ",
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      title="Quyết Định Bổ Nhiệm / Thay Đổi Chức Vụ"
      open={isOpen}
      onOk={handleSubmit}
      onCancel={onClose}
      confirmLoading={loading}
      okText="Xác nhận"
      cancelText="Hủy bỏ"
      destroyOnClose
      okButtonProps={{ className: "bg-blue-600 hover:bg-blue-700" }}
    >
      {employee && (
        <Alert
          message={
            <span className="font-semibold text-blue-800">
              Nhân sự: {employee.hoTen}
            </span>
          }
          description={`Mã NV (CCCD): ${employee.cccd} - Chức vụ hiện tại: ${employee.tenChucVu}`}
          type="info"
          showIcon
          className="mb-6 bg-blue-50 border-blue-200"
        />
      )}

      <Form form={form} layout="vertical">
        <Form.Item
          name="soQuyetDinh"
          label="Số quyết định"
          rules={[{ required: true, message: "Vui lòng nhập số quyết định!" }]}
        >
          <Input placeholder="VD: 126/QĐ-BN" />
        </Form.Item>

        <Form.Item
          name="idChucVuMoi"
          label="Chức vụ mới"
          rules={[{ required: true, message: "Vui lòng chọn chức vụ mới" }]}
        >
          <Select
            placeholder="-- Chọn chức vụ bổ nhiệm --"
            showSearch
            optionFilterProp="label"
            onChange={handlePositionChange}
            options={positions.map((p) => ({
              label: p.tenChucVu,
              value: p.idChucVu,
            }))}
          />
        </Form.Item>

        <Form.Item
          name="idBacLuongMoi"
          label="Bậc lương áp dụng (P1)"
          rules={[{ required: true, message: "Vui lòng chọn bậc lương" }]}
        >
          <Select
            placeholder="-- Chọn bậc lương --"
            loading={loadingSteps}
            disabled={salarySteps.length === 0}
            options={salarySteps.map((s) => ({
              label: `${s.stepName} - ${s.p1Salary.toLocaleString("vi-VN")} VNĐ`,
              value: s.id,
            }))}
            notFoundContent="Vui lòng chọn chức vụ trước"
          />
        </Form.Item>

        <Form.Item
          name="ngayHieuLuc"
          label="Ngày hiệu lực"
          rules={[{ required: true, message: "Vui lòng chọn ngày hiệu lực" }]}
        >
          <DatePicker format="DD/MM/YYYY" className="w-full" />
        </Form.Item>

        <Form.Item name="lyDo" label="Lý do bổ nhiệm / thăng tiến">
          <Input.TextArea rows={3} placeholder="Ghi chú lý do..." />
        </Form.Item>
      </Form>
    </Modal>
  );
};
