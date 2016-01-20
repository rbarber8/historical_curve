using MktSrvcAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class calib
    {
        public static void QuoteFiltering(byte[] exch, Quote[] bk, List<FilteredQuote> filtered, List<char> filteredExch)
        {
            if (exch == null)
            {
                filtered = new List<FilteredQuote>();
                filteredExch = new List<char>();
            }
            else if (exch.Count() > 4)
            {
                for (int i = 0; i < exch.Count(); i++)
                {
                    if (i == 0)
                    {
                        filtered.Add(new FilteredQuote
                        {
                            _prc = bk[i].prc,
                            _sz = bk[i].sz,
                        });
                        filteredExch.Add(Convert.ToChar(exch[i]));
                    }
                    else if (i <= 3)
                    {
                        //float deviation = Math.Abs(bk[i].prc - bk[0].prc);
                        float threshold = Math.Max(bk[i].prc / 10 * 4, 4);
                        float gap = bk[i].prc - bk[i - 1].prc;
                        //if (deviation > threshold)
                        //{
                        //    break;
                        //}
                        //else
                        //{
                        //    filtered.Add(new OrderBook
                        //    {
                        //        _prc = bk[i].prc,
                        //        _sz = bk[i].sz,
                        //    });
                        //    continue;
                        //}
                        if (gap > threshold)
                        {
                            break;
                        }
                        else
                        {
                            filtered.Add(new FilteredQuote
                            {
                                _prc = bk[i].prc,
                                _sz = bk[i].sz,
                            });
                            filteredExch.Add(Convert.ToChar(exch[i]));
                            continue;
                        }
                    }
                    else if (i > 3)
                    {
                        if (Math.Abs(bk[0].prc - bk[i].prc) > 0.1 && (Math.Abs(bk[0].prc - bk[i].prc) > (0.1 * bk[0].prc)))
                        {
                            float Mean = filtered.Select(x => x._prc).ToArray().Average();
                            float Deviation = Math.Abs(Mean - bk[i].prc);
                            float std = StandardDeviation(Mean, filtered);
                            if (Deviation < std)
                            {
                                filtered.Add(new FilteredQuote
                                {
                                    _prc = bk[i].prc,
                                    _sz = bk[i].sz,
                                });
                                filteredExch.Add(Convert.ToChar(exch[i]));
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            filtered.Add(new FilteredQuote
                            {
                                _prc = bk[i].prc,
                                _sz = bk[i].sz,
                            });
                            filteredExch.Add(Convert.ToChar(exch[i]));
                            continue;
                        }
                    }
                }
            }
            else if (exch.Count() <= 4)
            {
                for (int i = 0; i < exch.Count(); i++)
                {
                    if (i == 0)
                    {
                        filtered.Add(new FilteredQuote
                        {
                            _prc = bk[i].prc,
                            _sz = bk[i].sz,
                        });
                        filteredExch.Add(Convert.ToChar(exch[i]));
                    }
                    else
                    {
                        //float deviation = Math.Abs(bk[i].prc - bk[0].prc);
                        float threshold = Math.Max(bk[i].prc / 10 * 4, 4);
                        float gap = bk[i].prc - bk[i - 1].prc;
                        //if (deviation > threshold)
                        //{
                        //    break;
                        //}
                        //else
                        //{
                        //    filtered.Add(new OrderBook
                        //    {
                        //        _prc = bk[i].prc,
                        //        _sz = bk[i].sz,
                        //    });
                        //    continue;
                        //}
                        if (gap > threshold)
                        {
                            break;
                        }
                        else
                        {
                            filtered.Add(new FilteredQuote
                            {
                                _prc = bk[i].prc,
                                _sz = bk[i].sz,
                            });
                            filteredExch.Add(Convert.ToChar(exch[i]));
                            continue;
                        }
                    }
                }
            }
        }// Quote filtering rules

        public static float StandardDeviation(float mean, List<FilteredQuote> filtered)
        {
            float returnValue = 0.0f;
            foreach (float value in filtered.Select(x => x._prc).ToArray())
            {
                returnValue += (float)Math.Pow((value - mean), 2);
            }
            returnValue = (float)Math.Sqrt(returnValue / (filtered.Select(x => x._prc).ToArray().Count() - 1));
            return returnValue;
        }// Calculate standard deviation

        public static uint CalculateWsz(List<FilteredQuote> _prcbk)
        {
            uint sz = 0;
            if (_prcbk != null && _prcbk.Count > 0)
            {
                for (int i = 0; i < _prcbk.Count; ++i)
                {
                    sz += _prcbk[i]._sz;
                }
            }
            return sz;
        }

        public static float CalculateWprc(List<FilteredQuote> _prcbk)
        {
            float product = 0;
            if (_prcbk.Count != 0)
            {
                for (int i = 0; i < _prcbk.Count; ++i)
                {
                    product += _prcbk[i]._sz * _prcbk[i]._prc;
                }
            }
            return product;
        }

        public static float CalculateVWAP(List<FilteredQuote> _Bidbk, List<FilteredQuote> _Askbk)
        {
            float VWAP = 0.0f;
            if (_Bidbk.Count != 0 && _Askbk.Count != 0)
            {
                int n = ((_Bidbk.Count - _Askbk.Count) >= 0) ? _Askbk.Count : _Bidbk.Count;
                float sum_1 = 0.0f;//sumproduct of bidsz and askprc
                float sum_2 = 0.0f;//sumproduct of asksz and bidprc
                uint sum_3 = 0;//sum of bidsz
                uint sum_4 = 0;//sum of asksz
                for (int i = 0; i < n; ++i)
                {
                    sum_1 += _Bidbk[i]._sz * _Askbk[i]._prc;
                    sum_2 += _Askbk[i]._sz * _Bidbk[i]._prc;
                    sum_3 += _Bidbk[i]._sz;
                    sum_4 += _Askbk[i]._sz;
                }
                VWAP = (sum_1 + sum_2) / (sum_3 + sum_4);
            }
            else
            {
                VWAP = 0.0f;
            }
            return VWAP;
        }
    }
}
