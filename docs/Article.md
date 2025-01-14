.Net Client для облака mail.ru
Написано: 2016-04-11 в 12:49
Рубрики: .net, api, Облако Mail.ru
Метки: mail.ru api*

По заголовку статьи я думаю уже все догадались о чем будет идти речь. Как известно официального API для облака mail.ru пока не существует, поэтому попробуем написать его сами. Речь конечно же будет идти о клиентской части, а не о серверной.

Что было использовано и что необходимо знать для его написания:

— Язык программирования C#
— Среда разработки Visual Studio
— Базовые знания POST и GET запросов
— Браузер

Собственно, на что все это будет упираться, наше API всего лишь будет повторять некоторые запросы браузера к серверу cloud.mail.ru. При совершении любой операции с файлами или папками на облаке, браузер будет отправлять http запросы к серверу, на что тот будет отвечать и выдавать нужный вам ответ.

Самый простой пример запроса на удаление файла или папки. Я буду использовать Chrome Браузер для отслеживания запросов, где:

— Выбираем файл или папку которую хотим удалить
— Нажимаем F12
— Переходим во вкладку Network
— Нажимаем «Удалить» на файле или папке которую мы выбрали
— Во вкладке Network мы видим запросы, среди которых видим запрос, который называется remove

Remove Request
General
Request URL:https://cloud.mail.ru/api/v2/file/remove
Request Method:POST
Status Code:200 OK
Remote Address:217.69.139.7:443
Response Headers

Cache-Control:no-store, no-cache, must-revalidate
Connection:close
Content-Length:96
Content-Security-Policy-Report-Only:default-src *.cloud.mail.ru *.cloud.mail.ru *.datacloudmail.ru *.cldmail.ru *.mail.ru *.imgsmail.ru *.files.attachmail.ru *.mradx.net *.gemius.pl *.weborama.fr *.adriver.ru *.serving-sys.com featherservices.aviary.com d42hh4005hpu.cloudfront.net dme0ih8comzn4.cloudfront.net feather-client-files-aviary-prod-us-east-1.s3.amazonaws.com ; script-src 'unsafe-inline' 'unsafe-eval' *.cloud.mail.ru *.datacloudmail.ru *.cldmail.ru *.mail.ru *.imgsmail.ru *.files.attachmail.ru *.mradx.net *.yandex.ru *.odnoklassniki.ru odnoklassniki.ru *.ok.ru ok.ru *.scorecardresearch.com www.google-analytics.com featherservices.aviary.com d42hh4005hpu.cloudfront.net dme0ih8comzn4.cloudfront.net feather-client-files-aviary-prod-us-east-1.s3.amazonaws.com; img-src data: *; style-src 'unsafe-inline' *.mail.ru *.imgsmail.ru *.files.attachmail.ru *.mradx.net featherservices.aviary.com d42hh4005hpu.cloudfront.net dme0ih8comzn4.cloudfront.net feather-client-files-aviary-prod-us-east-1.s3.amazonaws.com; font-src data: cloud.mail.ru *.imgsmail.ru *.files.attachmail.ru *.mradx.net featherservices.aviary.com d42hh4005hpu.cloudfront.net dme0ih8comzn4.cloudfront.net feather-client-files-aviary-prod-us-east-1.s3.amazonaws.com; frame-src *.mail.ru *.datacloudmail.ru *.cldmail.ru docs.mail.ru *.officeapps.live.com *.mradx.net; object-src data: blob: https://*; report-uri https://cspreport.mail.ru/cloud/;
Content-Type:application/json; charset=utf-8
Date:Sun, 10 Apr 2016 13:43:38 GMT
Expires:Sat, 11 Apr 2015 13:43:38 GMT
Pragma:no-cache
Server:Tengine
Strict-Transport-Security:max-age=15768000; includeSubDomains; preload
X-Content-Type-Options:nosniff
X-Frame-Options:SAMEORIGIN
X-Host:clof8.i.mail.ru
X-req-id:OljB4ucgKW
X-server:api
X-UA-Compatible:IE=Edge
X-Upstream-Time:1460295818.308
X-XSS-Protection:1; mode=block; report=https://cspreport.mail.ru/xxssprotection
Request Headers

Accept:*/*
Accept-Encoding:gzip, deflate
Accept-Language:ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4
Connection:keep-alive
Content-Length:181
Content-Type:application/x-www-form-urlencoded; charset=UTF-8
Cookie:мои куки, выкладывать не стану
Host:cloud.mail.ru
Origin:https://cloud.mail.ru
Referer:https://cloud.mail.ru/home/
User-Agent:Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.112 Safari/537.36
X-Requested-With:XMLHttpRequest
Form Data

view URL encoded
home:/rename folder test (1)
api:2
build:hotfix-35-1.201604041616
x-page-id:DcpfqVgGGm
email:erastmorgan@bk.ru
x-email:erastmorgan@bk.ru
token:9qFgR48wHY5Rxqot4Ei515TiQMYCsPxD
Этот же самый запрос нам необходимо повторить из нашего API. Для этого мы будет использовать HttpWebRequest и HttpWebResponse классы. Здесь запрос должен выглядеть точно также, как и составил наш браузер выше, исключением являются некоторые заголовки, которые не являются обязательными. Пример запроса на удаления файла или папки смотрите ниже.

Remove Request C#
Этот код я выдернул прямо из своего проекта. Он довольно простой и не думаю что требует комментариев.
/// <summary>
        /// Remove file or folder.
        /// </summary>
        /// <param name="name">File or folder name.</param>
        /// <param name="fullPath">Full file or folder name.</param>
        private void Remove(string name, string fullPath)
        {
            var removeRequest = Encoding.UTF8.GetBytes(string.Format("home={0}&api={1}&token={2}&email={3}&x-email={3}", fullPath, 2, this.Account.AuthToken, this.Account.LoginName));

            var url = new Uri(string.Format("{0}/api/v2/file/remove", ConstSettings.CloudDomain));
            var request = (HttpWebRequest)WebRequest.Create(url.OriginalString);
            request.Proxy = this.Account.Proxy;
            request.CookieContainer = this.Account.Cookies;
            request.Method = "POST";
            request.ContentLength = removeRequest.LongLength;
            request.Referer = string.Format("{0}/home{1}", ConstSettings.CloudDomain, fullPath.Substring(0, fullPath.LastIndexOf(name)));
            request.Headers.Add("Origin", ConstSettings.CloudDomain);
            request.Host = url.Host;
            request.ContentType = ConstSettings.DefaultRequestType;
            request.Accept = "*/*";
            request.UserAgent = ConstSettings.UserAgent;
            using (var s = request.GetRequestStream())
            {
                s.Write(removeRequest, 0, removeRequest.Length);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception();
                    }
                }
            }
        }
Для любой другой операции в облаке mail.ru мы повторяем вышеуказанные действия, подставляя наши данные в место тех что вы словили из браузера.

Привожу список методов которые я написал в своем API:

— Create folder
— Copy file
— Copy folder
— Download file (Async operation)
— Upload file (Async operation)
— Get list of the files and folders
— Get public file link
— Get public folder link
— Get direct file link (operation on one session)
— Move file
— Move folder
— Rename file
— Rename folder
— Remove file
— Remove folder

API которое я написал вы можете скачать по ссылке. Отправляйте Pull Request если у вас есть время и вы хотите дополнить данное API новыми возможностями. Написано все это «на коленке», но главное работает и может быть свободно использовано в ваших проектах.

В будущем это API еще будет дополняться, к примеру я бы еще хотел добавить шифрование файлов и загрузку больших файлов (сейчас ограничение в 2GB), с таким условием что большой файл будет разбиваться на максимально возможные части при загрузке на облако, а при выгрузке обратно собирался.

Автор: erastmorgan
Источник: https://www.pvsm.ru/net/117611 
