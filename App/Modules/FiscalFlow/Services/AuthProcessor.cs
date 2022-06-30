using System;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PosStoneNfce.API.Portal.App.Common.Controller;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models;
using FiscalFlow.Common.Services.Cryptography;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Request;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Models.Response;
using PosStoneNfce.API.Portal.App.Common.Utils;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Services;
using PosStoneNfce.API.Portal.Interfaces;
using PosStoneNfce.API.Portal.Model;
using PosStoneNfce.API.Portal.Configuration;
using PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Exceptions;

namespace PosStoneNfce.API.Portal.App.Modules.FiscalFlow.Services
{
    public class AuthProcessor : AbstractProcessor
    {
        public AuthProcessor(IDBRepository<PosStone> dbRepository): base(dbRepository) {
            _dbRepository = dbRepository;
        }

        public dynamic Login(StoneCodeRequest stoneCodeRequest) {
            string httpResponse = "";
            dynamic apiData = "";
            string url = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_AUTH_SERVER"] + "/Usuario/Autenticacao/Login";
            object response = new {};
            try 
            {
                EncryptRequest encryptRequest = new EncryptRequest();
                encryptRequest.text = stoneCodeRequest.pass;
                var encryptedPass = Encrypt(encryptRequest);;
                string stoneCode = stoneCodeRequest.stone_code;
                string user = stoneCodeRequest.user;
                object dataToFF = new {
                    Input = new {
                        Email = user,
                        Senha = encryptedPass.cypher
                    }
                };
                httpResponse = CurlService.Post(url, dataToFF, null);
                var res = ObjectExtensions.FromJsonStringToObject<dynamic>(httpResponse);
                if (ObjectExtensions.PropertyExist(res, "Data")) {
                    apiData = res.Data;
                    string bearer = apiData.Token.ToString();
                    var expires = DateExtensions.GetEndOfDayTimestamp();

                    TokenEntity tokenEntity = new TokenEntity();
                    tokenEntity.stone_code = Int32.Parse(stoneCode);
                    tokenEntity.usuario = user;
                    tokenEntity.bearer = bearer;
                    tokenEntity.expires = expires;
                    var saved = _dbRepository.Save(tokenEntity).Result;
                    if (saved.error == false) {
                        response = new { error = false, usuario = stoneCodeRequest.user, pass = stoneCodeRequest.pass, expires = expires, bearer = bearer };
                    }
                    else {
                        response = new { error = true, msg = url + " - User = " + stoneCodeRequest.user + ", pass = " + stoneCodeRequest.pass + " - PosStone Error: Erro no banco de dados " + saved.msg };
                    }
                }
                else {
                    response = new { error = true, usuario = stoneCodeRequest.user, pass = stoneCodeRequest.pass, msg = url + " - User = " + stoneCodeRequest.user + ", pass = " + stoneCodeRequest.pass + " - FiscalFlow Error: " + res.ToString() };
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = url + " - User = " + stoneCodeRequest.user + ", pass = " + stoneCodeRequest.pass + " - PosStone Error: Exception Error: " + e.ToString()};
            }

            return response;
        }

        public dynamic AuthMaster() 
        {
            StoneCodeRequest stoneCodeRequest = new StoneCodeRequest();
            stoneCodeRequest.stone_code = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_MASTER_STONE_CODE"];
            stoneCodeRequest.user = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_MASTER_USER"];
            stoneCodeRequest.pass = ApiConfig.Configuration["ApiDefinitions:FISCALFLOW_MASTER_PASS"];
            return this.Login(stoneCodeRequest);
        }

        public dynamic Encrypt(EncryptRequest encryptRequest) {
            object response = new {};
            try {
                var cypher = SimpleAESEncryptor.AesEncrypt(encryptRequest.text);
                response = new { error = false, cypher = cypher};
            }
            catch (Exception e) {
                response = new { error = true, msg = e.ToString() };
            }
            return response;
        }

        public dynamic IsLoggedIn(StoneCodeRequest stoneCodeRequest) {
            object response = new {};
            try {

                TokenEntity tokenEntity = new TokenEntity();
                tokenEntity.stone_code = Int32.Parse(stoneCodeRequest.stone_code);
                DataTable dt = _dbRepository.FindOneBy<TokenEntity>(tokenEntity.GetTableName(), tokenEntity.GetPkName(), tokenEntity.GetPkValue()).Result;
                if (dt.Rows.Count > 0) {
                    string timestamp = dt.Rows[0]["expires"].ToString();
                    bool isExpired = DateExtensions.DateTimestampIsExpired(Int32.Parse(timestamp));
                    string bearer = dt.Rows[0]["bearer"].ToString();
                    response = new { error = false, is_logged_in = !isExpired, bearer = bearer};
                }
                else {
                    response = new { error = false, is_logged_in = false, bearer = ""};
                }
            }
            catch (Exception e) {
                response = new { error = true, msg = e.ToString() };
            }
            return response;
        }

    }
}