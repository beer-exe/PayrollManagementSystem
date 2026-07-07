export interface SalaryStepDto {
  id: string;
  stepName: string;
  p1Salary: number;
  effectiveDate: string;
  endDate?: string;
  status: 'HIEU_LUC' | 'HET_HIEU_LUC' | 'CHUA_AP_DUNG';
}

export interface CreateSalaryStepCommand 
{ 
    jobGradeId: string; 
    stepName: string; 
    p1Salary: number; 
    effectiveDate: string;
 }
export interface UpdateSalaryStepVersionCommand 
{ 
    jobGradeId: string; 
    stepName: string; 
    newP1Salary: number; 
    newEffectiveDate: string; 
}