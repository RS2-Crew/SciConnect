import { Injectable } from '@angular/core';

interface TranslationKeys {
  [key: string]: {
    en: string;
    sr: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private currentLanguage: 'en' | 'sr' = 'en';

  private translations: TranslationKeys = {
    'LOGOUT': {
      en: 'Logout',
      sr: 'Odjavi se'
    },
    'SEARCH_PLACEHOLDER': {
      en: 'Search for institutions, analyses, researchers, keywords...',
      sr: 'Pretraži ustanove, analize, istraživače, ključne reči...'
    },
    'ANALYSES_INSTITUTIONS': {
      en: 'Analyses & Institutions',
      sr: 'Analize & Ustanove'
    },
    'RESEARCHERS_KEYWORDS': {
      en: 'Researchers & Keywords',
      sr: 'Istraživači & Ključne Reči'
    },
    'GUIDED_SEARCH': {
      en: 'Guided Search',
      sr: 'Vođena Pretraga'
    },
    'INSTITUTION': {
      en: 'Institution',
      sr: 'Ustanova'
    },
    'ANALYSIS': {
      en: 'Analysis',
      sr: 'Analiza'
    },
    'MICROORGANISM': {
      en: 'Microorganism',
      sr: 'Mikroorganizam'
    },
    'INSTRUMENT': {
      en: 'Instrument',
      sr: 'Instrument'
    },
    'RESEARCHER': {
      en: 'Researcher',
      sr: 'Istraživač'
    },
    'KEYWORDS': {
      en: 'Keywords',
      sr: 'Ključne Reči'
    },
    'PIN_CATEGORY': {
      en: 'Pin Category',
      sr: 'Zakucaj Kategoriju'
    },
    'SELECT_INSTITUTION': {
      en: 'Select Institution',
      sr: 'Izaberi Ustanovu'
    },
    'SELECT_ANALYSIS': {
      en: 'Select Analysis',
      sr: 'Izaberi Analizu'
    },
    'SELECT_MICROORGANISM': {
      en: 'Select Microorganism',
      sr: 'Izaberi Mikroorganizam'
    },
    'SELECT_INSTRUMENT': {
      en: 'Select Instrument',
      sr: 'Izaberi Instrument'
    },
    'SELECT_RESEARCHER': {
      en: 'Select Researcher',
      sr: 'Izaberi Istraživača'
    },
    'SELECT_KEYWORD': {
      en: 'Select Keyword',
      sr: 'Izaberi Ključnu Reč'
    },
    'CLEAR_FILTERS': {
      en: 'Clear All Filters',
      sr: 'Očisti Sve Filtere'
    },
    'SEARCH_RESULTS': {
      en: 'Search Results',
      sr: 'Rezultati Pretrage'
    },
    'INSTITUTIONS': {
      en: 'Institutions',
      sr: 'Ustanove'
    },
    'ANALYSES': {
      en: 'Analyses',
      sr: 'Analize'
    },
    'RESEARCHERS': {
      en: 'Researchers',
      sr: 'Istraživači'
    },
    'DETAILS': {
      en: 'Details',
      sr: 'Detalji'
    },
    'PERFORMED_AT': {
      en: 'Performed at',
      sr: 'Izvršava se u'
    }
  };

  setLanguage(language: 'en' | 'sr'): void {
    this.currentLanguage = language;
  }

  getCurrentLanguage(): 'en' | 'sr' {
    return this.currentLanguage;
  }

  translate(key: string): string {
    const translation = this.translations[key];
    if (translation) {
      return translation[this.currentLanguage];
    }
    return key; // Return the key if translation not found
  }
}
