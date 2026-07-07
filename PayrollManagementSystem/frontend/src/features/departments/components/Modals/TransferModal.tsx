import React, { useEffect, useState } from "react";
import { Modal, Form, Select, DatePicker, Input, message, Alert } from "antd";
import { departmentApi } from "../../api/departmentApi";
import {
  DepartmentDto,
  EmployeeInDepartmentDto,
} from "../../types/department.types";
import { PositionDto } from "@/features/positions/types/position.types";
import { salaryStepApi } from "@/features/salarySteps/api/salaryStepApi";
import { SalaryStepDto } from "@/features/salarySteps/types/salaryStep.types";

interface TransferModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  departments: DepartmentDto[];
  positions: PositionDto[];
  employee: EmployeeInDepartmentDto | null;
}

export const TransferModal: React.FC<TransferModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
  departments,
  positions,
  employee,
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

    const selectedPos = positions.find((p) => p.idChucVu === positionId);
    if (!selectedPos || !selectedPos.idNgachLuong) {
      setSalarySteps([]);
      message.warning("Chức vụ này chưa được cấu hình ngạch lương.");
      return;
    }

    setLoadingSteps(true);
    try {
      const res = await salaryStepApi.getActive(selectedPos.idNgachLuong);
      if (res.succeeded) {
        setSalarySteps(res.data);
      }
    } catch (error) {
      message.error("Không thể tải danh sách bậc lương cho chức vụ này.");
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
        idPbMoi: values.idPbMoi,
        idChucVuMoi: values.idChucVuMoi,
        idBacLuongMoi: values.idBacLuongMoi,
        ngayHieuLuc: values.ngayHieuLuc.format("YYYY-MM-DD"),
        lyDo: values.lyDo,
      };

      const res = await departmentApi.transferEmployee(payload);
      if (res.succeeded) {
        message.success("Điều chuyển nhân sự thành công!");
        onSuccess();
        onClose();
      }
    } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
      if (err.response) {
        message.error(err.response.data.Message || "Lỗi khi điều chuyển");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      title="Quyết Định Điều Chuyển Nhân Sự"
      open={isOpen}
      onOk={handleSubmit}
      onCancel={onClose}
      confirmLoading={loading}
      okText="Xác nhận điều chuyển"
      cancelText="Hủy bỏ"
      destroyOnClose
      okButtonProps={{ className: "bg-violet-600 hover:bg-violet-700" }}
    >
      {employee && (
        <Alert
          message={
            <span className="font-semibold text-violet-800">
              Nhân sự: {employee.hoTen}
            </span>
          }
          description={`Mã NV (CCCD): ${employee.cccd} - Đang làm việc tại: ${employee.tenChucVu}`}
          type="info"
          showIcon
          className="mb-6 bg-violet-50 border-violet-200"
        />
      )}

      <Form form={form} layout="vertical">
        {/* ĐÃ THÊM Ô NHẬP SỐ QUYẾT ĐỊNH VÀO GIAO DIỆN */}
        <Form.Item
          name="soQuyetDinh"
          label="Số quyết định"
          rules={[{ required: true, message: "Vui lòng nhập số quyết định!" }]}
        >
          <Input placeholder="VD: 123/QĐ-NS" />
        </Form.Item>

        <Form.Item
          name="idPbMoi"
          label="Phòng ban mới"
          rules={[{ required: true, message: "Vui lòng chọn phòng ban mới" }]}
        >
          <Select
            placeholder="-- Chọn phòng ban --"
            showSearch
            optionFilterProp="label"
            options={departments.map((d) => ({
              label: d.tenPb,
              value: d.idPb,
            }))}
          />
        </Form.Item>

        <Form.Item
          noStyle
          shouldUpdate={(prevValues, currentValues) => prevValues.idPbMoi !== currentValues.idPbMoi}
        >
          {({ getFieldValue }) => (
            <Form.Item
              name="idChucVuMoi"
              label="Chức vụ mới"
              rules={[{ required: true, message: "Vui lòng chọn chức vụ mới" }]}
            >
              <Select
                placeholder="-- Chọn chức vụ --"
                showSearch
                optionFilterProp="label"
                onChange={handlePositionChange}
                disabled={!getFieldValue("idPbMoi")}
                options={positions
                  .filter((p) => p.idPhongBan === getFieldValue("idPbMoi"))
                  .map((p) => ({
                    label: p.tenChucVu,
                    value: p.idChucVu,
                  }))}
              />
            </Form.Item>
          )}
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
            notFoundContent="Vui lòng chọn chức vụ trước hoặc chức vụ này chưa cấu hình bậc lương"
          />
        </Form.Item>

        <Form.Item
          name="ngayHieuLuc"
          label="Ngày hiệu lực"
          rules={[{ required: true, message: "Vui lòng chọn ngày hiệu lực" }]}
        >
          <DatePicker format="DD/MM/YYYY" className="w-full" />
        </Form.Item>

        <Form.Item name="lyDo" label="Lý do điều chuyển">
          <Input.TextArea rows={3} placeholder="Ghi chú lý do..." />
        </Form.Item>
      </Form>
    </Modal>
  );
};
