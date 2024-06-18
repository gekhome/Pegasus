using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pegasus.DAL;
using Pegasus.Models;
using Pegasus.BPM;

namespace Pegasus.BPM
{
    public static class AitisiMoria
    {
        const int EPIMORFOSH1_MAX_HOURS = 200;
        const int EPIMORFOSH2_MAX_HOURS = 200;
        const int EPIMORFOSH3_MAX_HOURS = 200;

        //const int EPIMORFOSH1_MAX_HOURS_OLD = 300;
        //const int EPIMORFOSH2_MAX_HOURS_OLD = 100;
        //const int EPIMORFOSH3_MAX_HOURS_OLD = 100;

        const double MORIA_WORK_MAX = 10.0;


        #region ΜΟΡΙΑ ΑΙΤΗΣΗΣ (2018-2019)

        // ΜΟΡΙΑ ΒΑΣΙΚΟΥ ΠΤΥΧΙΟΥ (ΔΕΝ ΕΙΝΑΙ ΕΤΟΙΜΟ!)
        public static float MoriaDegree(AITISIS aitisi)
        {
            float moria_pe = 9.0f;
            float moria_aks = 6.0f;
            float moria_tede = 5.0f;
            float moria_et = 0.0f;

            int level = aitisi.PTYXIO_TYPE ?? 0;
            float moria_degree = 0.0f;

            // Εδώ πρέπει να μπουν οι ΤΕ με μόρια 6 (συγκεκριμένες ειδικότητες)
            if (!(level > 0)) 
                return moria_degree;

            if (level == 1)
                moria_degree = moria_pe;
            else if (level == 2)
                moria_degree = moria_aks;
            else if (level == 3 || level == 4)
                moria_degree = moria_tede;
            else if (level == 5)
                moria_degree = moria_et;

            return moria_degree;
        }

        // ΜΟΡΙΑ ΜΕΤΑΠΤΥΧΙΑΚΟΥ ΓΕΝΙΚΟΥ
        public static float MoriaMsc(AITISIS aitisi)
        {
            float moria_msc = 4.0f;
            float moria_postgraduate = 0.0f;

            bool msc = aitisi.MSC ?? false;
            if (msc) moria_postgraduate = moria_msc;

            return moria_postgraduate;
        }

        // ΜΟΡΙΑ ΔΙΔΑΚΤΟΡΙΚΟΥ ΓΕΝΙΚΟΥ
        public static float MoriaPhd(AITISIS aitisi)
        {
            float moria_phd = 7.0f;
            float moria_postgraduate = 0.0f;

            bool phd = aitisi.PHD ?? false;
            if (phd) moria_postgraduate = moria_phd;

            return moria_postgraduate;
        }

        // ΜΟΡΙΑ ΜΕΤΑΠΤΥΧΙΑΚΟΥ ΕΚΠ. ΕΝΗΛΙΚΩΝ
        public static float MoriaAedMsc(AITISIS aitisi)
        {
            float moria_aed_msc = 6.0f;
            float moria_value = 0.0f;

            bool aed_msc = aitisi.AED_MSC ?? false;
            if (aed_msc) moria_value = moria_aed_msc;

            return moria_value;
        }

        // ΜΟΡΙΑ ΔΙΔΑΚΤΟΡΙΚΟΥ ΕΚΠ. ΕΝΗΛΙΚΩΝ
        public static float MoriaAedPhd(AITISIS aitisi)
        {
            float moria_aed_phd = 9.0f;
            float moria_value = 0.0f;

            bool aed_phd = aitisi.AED_PHD ?? false;
            if (aed_phd) moria_value = moria_aed_phd;

            return moria_value;
        }

        public static float MoriaPostGrad(AITISIS aitisi)
        {
            float moriaMSC = Common.Max(MoriaMsc(aitisi), MoriaAedMsc(aitisi));
            float moriaPHD = Common.Max(MoriaPhd(aitisi), MoriaAedPhd(aitisi));

            float moriaPOSTGRAD = Common.Max(moriaMSC, moriaPHD);
            return moriaPOSTGRAD;
        }

        // ΜΟΡΙΑ ΕΠΙΜΟΡΦΩΣΗΣ ΣΤΟ ΔΙΔΑΚΤΙΚΟ ΑΝΤΙΚΕΙΜΕΝΟ
        public static float MoriaEpimorfosi1(AITISIS aitisi)
        {
            float moria_hour;
            float MORIA_MAX = 2.0f;

            bool epimorfosi = aitisi.EPIMORFOSI1 ?? false;

            if (aitisi.KLADOS < 4) moria_hour = 0.01f;
            else moria_hour = 0.01f;

            int hours = Math.Min(aitisi.EPIMORFOSI1_HOURS ?? 0, EPIMORFOSH1_MAX_HOURS);

            float moria_epimorfosi = 0.0f;

            if (!(epimorfosi == true)) return moria_epimorfosi;
            else moria_epimorfosi = moria_hour * (float)hours;
            return Common.Min(moria_epimorfosi, MORIA_MAX);
        }

        // ΜΟΡΙΑ ΕΠΙΜΟΡΦΩΣΗΣ ΣΤΗΝ ΕΠΑΓ. ΕΚΠΑΙΔΕΥΣΗ & ΚΑΤΑΡΤΙΣΗ
        public static float MoriaEpimorfosi2(AITISIS aitisi)
        {
            float MORIA_MAX = 2.0f;

            bool epimorfosi = aitisi.EPIMORFOSI2 ?? false;
            float moria_hour = 0.01f;
            int hours = Math.Min(aitisi.EPIMORFOSI2_HOURS ?? 0, EPIMORFOSH2_MAX_HOURS);
            float moria_epimorfosi = 0.0f;

            if (!(epimorfosi == true)) return moria_epimorfosi;
            else moria_epimorfosi = moria_hour * (float)hours;
            return Common.Min(moria_epimorfosi, MORIA_MAX);
        }

        // ΜΟΡΙΑ ΕΠΙΜΟΡΦΩΣΗΣ ΣΤΙΣ ΑΡΧΕΣ ΕΚΠ. ΕΝΗΛΙΚΩΝ
        public static float MoriaEpimorfosi3(AITISIS aitisi)
        {
            float moria_hour;
            float MORIA_MAX = 2.0f;

            bool epimorfosi = aitisi.EPIMORFOSI3 ?? false;

            if (aitisi.KLADOS < 4) moria_hour = 0.01f;
            else moria_hour = 0.01f;

            int hours = Math.Min(aitisi.EPIMORFOSI3_HOURS ?? 0, EPIMORFOSH3_MAX_HOURS);

            float moria_epimorfosi = 0.0f;

            if (!(epimorfosi == true)) return moria_epimorfosi;
            else moria_epimorfosi = moria_hour * (float)hours;
            return Common.Min(moria_epimorfosi, MORIA_MAX);
        }

        // ΜΟΡΙΑ ΠΑΙΔΑΓΩΓΙΚΟΥ ( = 0)
        public static float MoriaPed(AITISIS aitisi)
        {
            float moria_ped = 0.0f;
            float moria_pedagogic = 0.0f;

            bool ped = aitisi.PED ?? false;
            if (ped) moria_pedagogic = moria_ped;

            return moria_pedagogic;
        }

        // ΜΟΡΙΑ ΓΛΩΣΣΩΝ (2)
        public static float MoriaLanguage(AITISIS aitisi)
        {
            string lang1_text = aitisi.LANG1_TEXT;
            string lang1_level = aitisi.LANG1_LEVEL;
            string lang2_text = aitisi.LANG2_TEXT;
            string lang2_level = aitisi.LANG2_LEVEL;

            float moria_lang1 = 0.0f;
            float moria_lang2 = 0.0f;

            if (!string.IsNullOrEmpty(lang1_text))
            {
                if (lang1_level == "B2") moria_lang1 = 1.0f;
                else if (lang1_level == "C1") moria_lang1 = 1.5f;
                else if (lang1_level == "C2") moria_lang1 = 2.0f;
            }
            if (!string.IsNullOrEmpty(lang2_text))
            {
                if (lang2_level == "B2") moria_lang2 = 0.50f;
                else if (lang2_level == "C1") moria_lang2 = 0.75f;
                else if (lang2_level == "C2") moria_lang2 = 1.00f;
            }
            float moria_lang = Common.Min(moria_lang1 + moria_lang2, 3.0f);
            return moria_lang;
        }

        // ΜΟΡΙΑ ΓΝΩΣΕΩΝ Η/Υ
        public static float MoriaComputer(AITISIS aitisi)
        {
            int cert = aitisi.COMPUTER_CERT ?? 0;
            float moria_cert = 3.0f;
            float moria_computer = 0.0f;

            if (!(cert > 0)) return moria_computer;
            else return moria_cert;
        }

        // ΜΟΡΙΑ ΑΝΕΡΓΙΑΣ
        public static float MoriaAnergia(AITISIS aitisi)
        {
            float moria_anergia = 0.0f;
            float moria_base = 0.02f;

            int index_anergia = aitisi.ANERGIA ?? 0;
            if (index_anergia == 0)
            {
                return moria_anergia;
            }
            if (index_anergia == 1)
            {
                moria_anergia = moria_base * MoriaSum1(aitisi);
                return moria_anergia;
            }
            else
            {
                moria_anergia = (moria_base + (index_anergia - 1) * moria_base) * MoriaSum1(aitisi);
                return moria_anergia;
            }
        }

        // ΜΟΡΙΑ ΚΟΙΝΩΝΙΚΑ ΚΡΙΤΗΡΙΑ
        public static float MoriaSocial(AITISIS aitisi)
        {
            float percent_group1 = 0.0f;
            float percent_group2 = 0.0f;
            float percent_group3 = 0.0f;
            float percent_group4 = 0.0f;

            if (aitisi.SOCIALGROUP1 == true) percent_group1 = 0.10f;
            if (aitisi.SOCIALGROUP2 == true) percent_group2 = 0.10f;
            if (aitisi.SOCIALGROUP3 == true) percent_group3 = 0.10f;
            if (aitisi.SOCIALGROUP4 == true) percent_group4 = 0.10f;

            float moria_social = (percent_group1 + percent_group2 + percent_group3 + percent_group4) * MoriaSum(aitisi);
            return moria_social;
        }

        // ΜΟΡΙΑ ΔΙΔΑΚΤΙΚΗΣ ΕΜΠΕΙΡΙΑΣ
        public static float MoriaTeach(AITISIS aitisi)
        {
            return aitisi.MORIA_TEACH ?? 0f;
        }

        // ΜΟΡΙΑ ΕΠΑΓΓΕΛΜΑΤΙΚΗΣ ΕΜΠΕΙΡΙΑΣ
        public static float MoriaWork1(AITISIS aitisi)
        {
            return aitisi.MORIA_WORK1 ?? 0f;
        }

        // ΜΟΡΙΑ ΕΛΕΥΘΕΡΟΥ ΕΠΑΓΓΕΛΜΑΤΟΣ
        public static float MoriaWork2(AITISIS aitisi)
        {
            return aitisi.MORIA_WORK2 ?? 0f;
        }

        public static float MoriaWork(AITISIS aitisi)
        {
            return aitisi.MORIA_WORK ?? 0f;
        }

        // ΑΘΡΟΙΣΜΑ ΜΟΡΙΩΝ ΕΚΤΟΣ ΜΟΡΙΩΝ ΑΝΕΡΓΙΑΣ ΚΑΙ ΜΟΡΙΩΝ ΚΟΙΝ. ΚΡΙΤΗΡΙΩΝ
        public static float MoriaSum1(AITISIS aitisi)
        {
            float m_degree = MoriaDegree(aitisi);
            float m_postgrad = MoriaPostGrad(aitisi);
            float m_pedagogic = MoriaPed(aitisi);
            float m_language = MoriaLanguage(aitisi);
            float m_computer = MoriaComputer(aitisi);
            float m_epimorfosi1 = MoriaEpimorfosi1(aitisi);
            float m_epimorfosi2 = MoriaEpimorfosi2(aitisi);
            float m_epimorfosi3 = MoriaEpimorfosi3(aitisi);
            float m_teach = MoriaTeach(aitisi);
            float m_work1 = MoriaWork1(aitisi);
            float m_work2 = MoriaWork2(aitisi);

            float m_work = Common.Min(m_work1 + m_work2, (float)MORIA_WORK_MAX);

            float moria_sum = m_degree + m_postgrad + m_pedagogic + m_language + m_computer + 
                              m_epimorfosi1 + m_epimorfosi2 + m_epimorfosi3 + m_teach + m_work;
            return moria_sum;
        }

        public static float MoriaSum(AITISIS aitisi)
        {
            float moria_sum = MoriaSum1(aitisi) + MoriaAnergia(aitisi);
            return moria_sum;
        }

        // ΑΘΡΟΙΣΜΑ ΜΟΡΙΩΝ ΜΕ ΤΑ ΚΟΙΝ. ΚΡΙΤΗΡΙΑ
        public static float MoriaTotal(AITISIS aitisi)
        {
            float moria_total = MoriaSum(aitisi) + MoriaSocial(aitisi);
            return moria_total;
        }

        #endregion ΜΟΡΙΑ ΑΙΤΗΣΗΣ (2018-2019)


        #region Moria Aitisi (2016-2018)

        public static float MoriaAnergiaOld2(AITISIS aitisi)
        {
            float moria_anergia = 0.0f;
            float moria_base = 4.0f;

            int index_anergia = aitisi.ANERGIA ?? 0;
            if (index_anergia == 0)
            {
                return moria_anergia;
            }
            if (index_anergia == 1)
            {
                moria_anergia = moria_base;
                return moria_anergia;
            }
            else
            {
                moria_anergia = Common.Min(moria_base + (index_anergia - 1) * 4.0f, 20.0f);
                return moria_anergia;
            }
        }

        public static float MoriaDegreeOld2(AITISIS aitisi)
        {
            float moria_pe = 12.0f;
            float moria_te = 6.0f;
            float moria_de = 6.0f;
            float moria_et = 6.0f;

            int levelDegree = aitisi.KLADOS ?? 0;
            float moria_degree = 0.0f;

            if (!(levelDegree > 0)) return moria_degree;

            if (levelDegree == 1)
            {
                moria_degree = moria_pe;
            }
            else if (levelDegree == 2)
            {
                moria_degree = moria_te;
            }
            else if (levelDegree == 3)
            {
                moria_degree = moria_de;
            }
            else if (levelDegree == 4)
            {
                moria_degree = moria_et;
            }
            return moria_degree;
        }

        public static float MoriaMscOld2(AITISIS aitisi)
        {
            float moria_msc = 5.0f;
            float moria_postgraduate = 0.0f;

            bool msc = aitisi.MSC ?? false;
            if (msc == true) moria_postgraduate = moria_msc;

            return moria_postgraduate;
        }

        public static float MoriaPhdOld2(AITISIS aitisi)
        {
            float moria_phd = 8.0f;
            float moria_postgraduate = 0.0f;

            bool phd = aitisi.PHD ?? false;
            if (phd == true) moria_postgraduate = moria_phd;

            return moria_postgraduate;
        }

        public static float MoriaPedOld2(AITISIS aitisi)
        {
            float moria_ped = 0.0f;
            float moria_pedagogic = 0.0f;

            bool ped = aitisi.PED ?? false;
            if (ped == true) moria_pedagogic = moria_ped;

            return moria_pedagogic;
        }

        public static float MoriaAedMscOld2(AITISIS aitisi)
        {
            float moria_aed_msc = 5.0f;
            float moria_value = 0.0f;

            bool aed_msc = aitisi.AED_MSC ?? false;
            if (aed_msc == true) moria_value = moria_aed_msc;

            return moria_value;
        }

        public static float MoriaAedPhdOld2(AITISIS aitisi)
        {
            float moria_aed_phd = 8.0f;
            float moria_value = 0.0f;

            bool aed_phd = aitisi.AED_PHD ?? false;
            if (aed_phd == true) moria_value = moria_aed_phd;

            return moria_value;
        }

        public static float MoriaLanguageOld2(AITISIS aitisi)
        {
            string lang1_text = aitisi.LANG1_TEXT;
            string lang1_level = aitisi.LANG1_LEVEL;
            string lang2_text = aitisi.LANG2_TEXT;
            string lang2_level = aitisi.LANG2_LEVEL;

            float moria_lang1 = 0.0f;
            float moria_lang2 = 0.0f;

            if (!string.IsNullOrEmpty(lang1_text))
            {
                if (lang1_level == "B2") moria_lang1 = 1.0f;
                else if (lang1_level == "C1") moria_lang1 = 1.5f;
                else if (lang1_level == "C2") moria_lang1 = 2.0f;
            }
            if (!string.IsNullOrEmpty(lang2_text))
            {
                if (lang2_level == "B2") moria_lang2 = 0.50f;
                else if (lang2_level == "C1") moria_lang2 = 0.75f;
                else if (lang2_level == "C2") moria_lang2 = 1.00f;
            }
            float moria_lang = moria_lang1 + moria_lang2;
            return moria_lang;
        }

        public static float MoriaComputerOld2(AITISIS aitisi)
        {
            int cert = aitisi.COMPUTER_CERT ?? 0;
            float moria_cert = 3.0f;
            float moria_computer = 0.0f;

            if (!(cert > 0)) return moria_computer;
            else return moria_cert;
        }

        public static float MoriaEpimorfosi1Old2(AITISIS aitisi)
        {
            float moria_hour;
            float MORIA_MAX = 2.0f;

            bool epimorfosi = aitisi.EPIMORFOSI1 ?? false;

            if (aitisi.KLADOS < 4) moria_hour = 0.01f;
            else moria_hour = 0.01f;

            int hours = Math.Min(aitisi.EPIMORFOSI1_HOURS ?? 0, EPIMORFOSH1_MAX_HOURS);
            
            float moria_epimorfosi = 0.0f;

            if (!(epimorfosi == true)) return moria_epimorfosi;
            else moria_epimorfosi = moria_hour * hours;
            return Common.Min(moria_epimorfosi, MORIA_MAX);
        }

        public static float MoriaEpimorfosi2Old2(AITISIS aitisi)
        {
            float MORIA_MAX = 2.0f;

            bool epimorfosi = aitisi.EPIMORFOSI2 ?? false;
            float moria_hour = 0.01f;
            int hours = Math.Min(aitisi.EPIMORFOSI2_HOURS ?? 0, EPIMORFOSH2_MAX_HOURS);
            float moria_epimorfosi = 0.0f;

            if (!(epimorfosi == true)) return moria_epimorfosi;
            else moria_epimorfosi = moria_hour * hours;
            return Common.Min(moria_epimorfosi, MORIA_MAX);
        }

        public static float MoriaEpimorfosi3Old2(AITISIS aitisi)
        {
            float moria_hour;
            float MORIA_MAX = 2.0f;

            bool epimorfosi = aitisi.EPIMORFOSI3 ?? false;

            if (aitisi.KLADOS < 4) moria_hour = 0.01f;
            else moria_hour = 0.01f;

            int hours = Math.Min(aitisi.EPIMORFOSI3_HOURS ?? 0, EPIMORFOSH3_MAX_HOURS);

            float moria_epimorfosi = 0.0f;

            if (!(epimorfosi == true)) return moria_epimorfosi;
            else moria_epimorfosi = moria_hour * hours;
            return Common.Min(moria_epimorfosi, MORIA_MAX);
        }

        public static float MoriaTeachOld2(AITISIS aitisi)
        {
            return aitisi.MORIA_TEACH ?? 0f;
        }

        public static float MoriaWork1Old2(AITISIS aitisi)
        {
            return aitisi.MORIA_WORK1 ?? 0f;
        }

        public static float MoriaWork2Old2(AITISIS aitisi)
        {
            return aitisi.MORIA_WORK2 ?? 0f;
        }

        public static float MoriaWorkOld2(AITISIS aitisi)
        {
            return aitisi.MORIA_WORK ?? 0f;
        }

        public static float MoriaSumOld2(AITISIS aitisi)
        {
            float m_anergia = MoriaAnergia(aitisi);
            float m_degree = MoriaDegree(aitisi);
            float m_postgrad = Common.Max(MoriaMsc(aitisi), MoriaPhd(aitisi));
            float m_pedagogic = MoriaPed(aitisi);
            float m_aed_postgrad = Common.Max(MoriaAedMsc(aitisi), MoriaAedPhd(aitisi));
            float m_language = MoriaLanguage(aitisi);
            float m_computer = MoriaComputer(aitisi);
            float m_epimorfosi1 = MoriaEpimorfosi1(aitisi);
            float m_epimorfosi2 = MoriaEpimorfosi2(aitisi);
            float m_epimorfosi3 = MoriaEpimorfosi3(aitisi);
            float m_teach = MoriaTeach(aitisi);
            float m_work1 = MoriaWork1(aitisi);
            float m_work2 = MoriaWork2(aitisi);

            float m_work = Common.Min(m_work1 + m_work2, (float)MORIA_WORK_MAX);

            float moria_sum = m_anergia + m_degree + m_postgrad + m_pedagogic +
                                 m_aed_postgrad + m_language + m_computer + m_epimorfosi1 + m_epimorfosi2 + m_epimorfosi3 +
                                 m_teach + m_work;
            return moria_sum;
        }

        public static float MoriaSocialOld2(AITISIS aitisi)
        {
            float percent_group1 = 0.0f;
            float percent_group2 = 0.0f;
            float percent_group3 = 0.0f;
            float percent_group4 = 0.0f;

            if (aitisi.SOCIALGROUP1 == true) percent_group1 = 0.10f;
            if (aitisi.SOCIALGROUP2 == true) percent_group2 = 0.10f;
            if (aitisi.SOCIALGROUP3 == true) percent_group3 = 0.10f;
            if (aitisi.SOCIALGROUP4 == true) percent_group4 = 0.10f;

            float moria_social = (percent_group1 + percent_group2 + percent_group3 + percent_group4)*MoriaSum(aitisi);
            return moria_social;
        }

        public static float MoriaTotalOld2(AITISIS aitisi)
        {
            float moria_total = MoriaSum(aitisi) + MoriaSocial(aitisi);

            return moria_total;
        }

        #endregion


        #region Moria Aitisi Old (2015-Old2)

        public static float MoriaAnergiaOld(AITISIS aitisi)
        {
            float moria_anergia = 0.0f;
            float moria_base = 6.0f;

            int index_anergia = aitisi.ANERGIA ?? 0;
            if (index_anergia == 0)
            {
                return moria_anergia;
            }
            if (index_anergia == 1)
            {
                moria_anergia = moria_base;
                return moria_anergia;
            }
            else
            {
                moria_anergia = moria_base + (index_anergia - 1) * 0.25f;
                return moria_anergia;
            }
        }

        public static float MoriaDegreeOld(AITISIS aitisi)
        {
            float moria_pe = 15.0f;
            float moria_te = 6.0f;
            float moria_de = 6.0f;
            float moria_et = 0.0f;

            int levelDegree = aitisi.KLADOS ?? 0;
            float moria_degree = 0.0f;

            if (!(levelDegree > 0)) return moria_degree;

            if (levelDegree == 1)
            {
                moria_degree = moria_pe;
            }
            else if (levelDegree == 2)
            {
                moria_degree = moria_te;
            }
            else if (levelDegree == 3)
            {
                moria_degree = moria_de;
            }
            else if (levelDegree == 4)
            {
                moria_degree = moria_et;
            }
            return moria_degree;
        }

        public static float MoriaMscOld(AITISIS aitisi)
        {
            float moria_msc = 5.0f;
            float moria_postgraduate = 0.0f;

            bool msc = aitisi.MSC ?? false;
            if (msc == true) moria_postgraduate = moria_msc;

            return moria_postgraduate;
        }

        public static float MoriaPhdOld(AITISIS aitisi)
        {
            float moria_phd = 7.0f;
            float moria_postgraduate = 0.0f;

            bool phd = aitisi.PHD ?? false;
            if (phd == true) moria_postgraduate = moria_phd;

            return moria_postgraduate;
        }

        public static float MoriaPedOld(AITISIS aitisi)
        {
            float moria_ped = 10.0f;
            float moria_pedagogic = 0.0f;

            bool ped = aitisi.PED ?? false;
            if (ped == true) moria_pedagogic = moria_ped;

            return moria_pedagogic;
        }

        public static float MoriaAedMscOld(AITISIS aitisi)
        {
            float moria_aed_msc = 4.0f;
            float moria_value = 0.0f;

            bool aed_msc = aitisi.AED_MSC ?? false;
            if (aed_msc == true) moria_value = moria_aed_msc;

            return moria_value;
        }

        public static float MoriaAedPhdOld(AITISIS aitisi)
        {
            float moria_aed_phd = 6.0f;
            float moria_value = 0.0f;

            bool aed_phd = aitisi.AED_PHD ?? false;
            if (aed_phd == true) moria_value = moria_aed_phd;

            return moria_value;
        }

        public static float MoriaLanguageOld(AITISIS aitisi)
        {
            string lang_text = aitisi.LANG_TEXT;
            string lang_level = aitisi.LANG_LEVEL;
            float moria_lang = 0.0f;

            if (string.IsNullOrEmpty(lang_text)) return moria_lang;

            if (lang_level == "B2") moria_lang = 1;
            else if (lang_level == "C1") moria_lang = 2;
            else if (lang_level == "C2") moria_lang = 3;

            return moria_lang;
        }

        public static float MoriaComputerOld(AITISIS aitisi)
        {
            int cert = aitisi.COMPUTER_CERT ?? 0;
            float moria_cert = 2.0f;
            float moria_computer = 0.0f;

            if (!(cert > 0)) return moria_computer;
            else return moria_cert;
        }

        public static float MoriaEpimorfosi1Old(AITISIS aitisi)
        {
            float moria_hour;
            float MORIA_MAX = 3.0f;

            bool epimorfosi = aitisi.EPIMORFOSI1 ?? false;

            if (aitisi.KLADOS < 4) moria_hour = 0.01f;
            else moria_hour = 0.02f;

            int hours = Math.Min(aitisi.EPIMORFOSI1_HOURS ?? 0, EPIMORFOSH1_MAX_HOURS);

            float moria_epimorfosi = 0.0f;

            if (!(epimorfosi == true)) return moria_epimorfosi;
            else moria_epimorfosi = moria_hour * hours;
            return Common.Min(moria_epimorfosi, MORIA_MAX);
        }

        public static float MoriaEpimorfosi2Old(AITISIS aitisi)
        {
            float MORIA_MAX = 2.0f;

            bool epimorfosi = aitisi.EPIMORFOSI2 ?? false;
            float moria_hour = 0.02f;
            int hours = Math.Min(aitisi.EPIMORFOSI2_HOURS ?? 0, EPIMORFOSH2_MAX_HOURS);
            float moria_epimorfosi = 0.0f;

            if (!(epimorfosi == true)) return moria_epimorfosi;
            else moria_epimorfosi = moria_hour * hours;
            return Common.Min(moria_epimorfosi, MORIA_MAX);
        }

        public static float MoriaTeachOld(AITISIS aitisi)
        {
            return aitisi.MORIA_TEACH ?? 0f;
        }

        public static float MoriaWork1Old(AITISIS aitisi)
        {
            return aitisi.MORIA_WORK1 ?? 0f;
        }

        public static float MoriaWork2Old(AITISIS aitisi)
        {
            return aitisi.MORIA_WORK2 ?? 0f;
        }

        public static float MoriaSumOld(AITISIS aitisi)
        {
            float m_anergia = MoriaAnergiaOld(aitisi);
            float m_degree = MoriaDegreeOld(aitisi);
            float m_postgrad = Common.Max(MoriaMscOld(aitisi), MoriaPhdOld(aitisi));
            float m_pedagogic = MoriaPedOld(aitisi);
            float m_aed_postgrad = Common.Max(MoriaAedMscOld(aitisi), MoriaAedPhdOld(aitisi));
            float m_language = MoriaLanguageOld(aitisi);
            float m_computer = MoriaComputerOld(aitisi);
            float m_epimorfosi1 = MoriaEpimorfosi1Old(aitisi);
            float m_epimorfosi2 = MoriaEpimorfosi2Old(aitisi);
            float m_teach = MoriaTeachOld(aitisi);
            float m_work1 = MoriaWork1Old(aitisi);
            float m_work2 = MoriaWork2Old(aitisi);

            float m_work = Common.Min(m_work1 + m_work2, (float)MORIA_WORK_MAX);

            float moria_sum = m_anergia + m_degree + m_postgrad + m_pedagogic +
                                 m_aed_postgrad + m_language + m_computer + m_epimorfosi1 + m_epimorfosi2 +
                                 m_teach + m_work;
            return moria_sum;
        }

        public static float MoriaSocialOld(AITISIS aitisi)
        {
            float moria_social = (0.10f) * MoriaSumOld(aitisi);

            int social_group = aitisi.SOCIALGROUP ?? 0;
            if (social_group > 0) return moria_social;
            else return 0;
        }

        public static float MoriaTotalOld(AITISIS aitisi)
        {
            float moria_total = MoriaSumOld(aitisi) + MoriaSocialOld(aitisi);

            return moria_total;
        }


        #endregion
    }
}