export interface SalaryStepDto {
  id: string;
  stepName: string;
  p1Salary: number;
  effectiveDate: string;
  endDate?: string;
  status: 'HIEU_LUC' | 'HET_HIEU_LUC';
}

export interface CreateSalaryStepCommand 
{ 
    positionId: string; 
    stepName: string; 
    p1Salary: number; 
    effectiveDate: string;
 }
export interface UpdateSalaryStepVersionCommand 
{ 
    positionId: string; 
    stepName: string; 
    newP1Salary: number; 
    newEffectiveDate: string; 
}