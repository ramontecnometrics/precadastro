using System;

namespace framework.Validators
{ 
    public static class CNSValidator
    {
        /// <summary>
        /// Verifica se o número CNS informado é definitivo ou provisório e se ele é válido [FB]
        /// </summary>
        /// <param name="cns">Número de CNS a ser checado</param>
        /// <returns>True, se o número é válido; False, se for inválido.</returns>
        public static bool chkNumeroCNS(string cns)
        {
            if (string.IsNullOrWhiteSpace(cns))
            {
                return true;
            }
            cns = cns.Trim();
            if (string.IsNullOrWhiteSpace(cns))
            {
                return true;
            }


            bool result = false;

            cns = cns.Trim().Replace(" ", "");


            if ((cns.Substring(0, 1) == "8") || (cns.Substring(0, 1) == "9"))
            {
                result = chkNumeroProvisorio(cns);
            }
            else
            {
                result = chkNumeroDefinitivo(cns);
            }

            return result;
        }


        /// <summary>
        /// Verifica se um número CNS provisório é válido [FB]
        /// </summary>
        /// <param name="cns">Número de CNS a ser checado</param>
        /// <returns>True, se o número é válido; False, se for inválido.</returns>
        private static bool chkNumeroProvisorio(string cns)
        {
            bool result = false;

            try
            {
                cns = cns.Trim();

                if (cns.Trim().Length == 15)
                {
                    float resto, soma;

                    soma = ((Convert.ToInt64(cns.Substring(0, 1))) * 15) +
                            ((Convert.ToInt64(cns.Substring(1, 1))) * 14) +
                            ((Convert.ToInt64(cns.Substring(2, 1))) * 13) +
                            ((Convert.ToInt64(cns.Substring(3, 1))) * 12) +
                            ((Convert.ToInt64(cns.Substring(4, 1))) * 11) +
                            ((Convert.ToInt64(cns.Substring(5, 1))) * 10) +
                            ((Convert.ToInt64(cns.Substring(6, 1))) * 9) +
                            ((Convert.ToInt64(cns.Substring(7, 1))) * 8) +
                            ((Convert.ToInt64(cns.Substring(8, 1))) * 7) +
                            ((Convert.ToInt64(cns.Substring(9, 1))) * 6) +
                            ((Convert.ToInt64(cns.Substring(10, 1))) * 5) +
                            ((Convert.ToInt64(cns.Substring(11, 1))) * 4) +
                            ((Convert.ToInt64(cns.Substring(12, 1))) * 3) +
                            ((Convert.ToInt64(cns.Substring(13, 1))) * 2) +
                            ((Convert.ToInt64(cns.Substring(14, 1))) * 1);

                    resto = soma % 11;

                    result = (resto == 0);
                }

            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Verifica se um número CNS definitivo é válido [FB]
        /// </summary>
        /// <param name="cns">Número de CNS a ser checado</param>
        /// <returns>True, se o número é válido; False, se for inválido.</returns>
        private static bool chkNumeroDefinitivo(string cns)
        {
            bool result = false;
            try
            {
                if (cns.Trim().Length == 15)
                {
                    float resto, soma, dv;

                    string pis = string.Empty;
                    string resultado = string.Empty;

                    pis = cns.Substring(0, 11);

                    soma = ((Convert.ToInt64(pis.Substring(0, 1))) * 15) +
                            ((Convert.ToInt64(pis.Substring(1, 1))) * 14) +
                            ((Convert.ToInt64(pis.Substring(2, 1))) * 13) +
                            ((Convert.ToInt64(pis.Substring(3, 1))) * 12) +
                            ((Convert.ToInt64(pis.Substring(4, 1))) * 11) +
                            ((Convert.ToInt64(pis.Substring(5, 1))) * 10) +
                            ((Convert.ToInt64(pis.Substring(6, 1))) * 9) +
                            ((Convert.ToInt64(pis.Substring(7, 1))) * 8) +
                            ((Convert.ToInt64(pis.Substring(8, 1))) * 7) +
                            ((Convert.ToInt64(pis.Substring(9, 1))) * 6) +
                            ((Convert.ToInt64(pis.Substring(10, 1))) * 5);


                    resto = soma % 11;
                    dv = 11 - resto;

                    if (dv == 11)
                    {
                        dv = 0;
                    }

                    if (dv == 10)
                    {
                        soma = ((Convert.ToInt64(pis.Substring(0, 1))) * 15) +
                                ((Convert.ToInt64(pis.Substring(1, 1))) * 14) +
                                ((Convert.ToInt64(pis.Substring(2, 1))) * 13) +
                                ((Convert.ToInt64(pis.Substring(3, 1))) * 12) +
                                ((Convert.ToInt64(pis.Substring(4, 1))) * 11) +
                                ((Convert.ToInt64(pis.Substring(5, 1))) * 10) +
                                ((Convert.ToInt64(pis.Substring(6, 1))) * 9) +
                                ((Convert.ToInt64(pis.Substring(7, 1))) * 8) +
                                ((Convert.ToInt64(pis.Substring(8, 1))) * 7) +
                                ((Convert.ToInt64(pis.Substring(9, 1))) * 6) +
                                ((Convert.ToInt64(pis.Substring(10, 1))) * 5) + 2;

                        resto = soma % 11;
                        dv = 11 - resto;
                        resultado = pis + "001" + Convert.ToString(Convert.ToInt16(dv)).Trim();
                    }
                    else
                    {
                        resultado = pis + "000" + Convert.ToString(Convert.ToInt16(dv)).Trim();
                    }
                    result = cns.Equals(resultado);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}