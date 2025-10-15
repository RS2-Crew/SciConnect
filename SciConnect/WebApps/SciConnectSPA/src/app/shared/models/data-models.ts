export interface Institution {
  id: number;
  name: string;
  description?: string;
  address?: string;
  city?: string;
  country?: string;
  phone?: string;
  email?: string;
  website?: string;
  analyses?: Analysis[];
  instruments?: Instrument[];
  employees?: Employee[];
  keywords?: Keyword[];
  microorganisms?: Microorganism[];
}

export interface Analysis {
  id: number;
  name: string;
  description?: string;
  type?: string;
  institutions?: Institution[];
  microorganisms?: Microorganism[];
}

export interface Instrument {
  id: number;
  name: string;
  description?: string;
  manufacturer?: string;
  model?: string;
  institutions?: Institution[];
}

export interface Keyword {
  id: number;
  name: string;
  description?: string;
  researchers?: Employee[];
}

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  title?: string;
  institution?: Institution;
  keywords?: Keyword[];
}

export interface Microorganism {
  id: number;
  name: string;
  description?: string;
  type?: string;
  analysis?: Analysis[];
  institution?: Institution;
}

export interface InstitutionViewModel {
  id: number;
  name: string;
  description?: string;
  address?: string;
  city?: string;
  country?: string;
  phone?: string;
  email?: string;
  website?: string;
}

export interface AnalysisViewModel {
  id: number;
  name: string;
  description?: string;
  type?: string;
  institutions?: InstitutionViewModel[];
  microorganisms?: MicroorganismViewModel[];
}

export interface InstrumentViewModel {
  id: number;
  name: string;
  description?: string;
  manufacturer?: string;
  model?: string;
}

export interface KeywordViewModel {
  id: number;
  name: string;
  description?: string;
  researchers?: EmployeeViewModel[];
}

export interface EmployeeViewModel {
  id: number;
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  title?: string;
  institution?: InstitutionViewModel;
  keywords?: KeywordViewModel[];
}

export interface MicroorganismViewModel {
  id: number;
  name: string;
  description?: string;
  type?: string;
}

export interface FilterState {
  selectedInstitution?: Institution;
  selectedInstruments: Instrument[];
  selectedAnalyses: Analysis[];
  selectedKeywords: Keyword[];
  selectedEmployees: Employee[];
  selectedMicroorganisms: Microorganism[];
}
