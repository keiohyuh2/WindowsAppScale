Imports System.Text
Imports System.Runtime.InteropServices
Imports Newtonsoft.Json
Imports System.Net
Imports System.IO.Ports


Public Class form1
    ' iniファイルの場所を定数として宣言
    Public Const INI_FILE_PATH As String = "./Scale.ini" '初期設定ファイル
    '浜田様サブドメイン
    Public Const SUB_DOMAIN = "kkhamada"  '参考：https://kkhamada.cybozu.com/k/485/

    'kintone関連
    'アプリID、kintone-APIのurl、トークンを、環境に合わせて設定
    '■１．kintoneアプリのapp_id
    'Public Const APP_ID = "3"  '問合せアプリ
    'テスト環境の計量機重量アプリID
    'Public Const APP_ID = "12"  '計量機重量アプリアプリ
    '浜田様環境の計量機重量アプリID
    Public Const APP_ID = "485"  '計量機重量アプリアプリ
    '■２．url
    ' kintoneアプリのURL 「複数」レコードGET
    'Public Const KINTONE_GET_URL As String = "https://devotigoz.cybozu.com/k/v1/records.json" 'テスト環境 URL
    Public Const KINTONE_GET_URL As String = "https://" & SUB_DOMAIN & ".cybozu.com/k/v1/records.json" 'kintone URL
    ' kintoneアプリのURL レコード更新
    Public Const KINTONE_UPD_URL As String = "https://" & SUB_DOMAIN & ".cybozu.com/k/v1/record.json" 'kintone URL
    ' 問合せアプリのトークン
    'Private Const SCALE_TOKEN As String = "v879Wvs6BjG5PDlg8z3QRXSOd1Y1B2Hao68RloSc" 'kintone 問合せアプリ
    'テスト環境の計量機重量アプリのトークン
    'Private Const SCALE_TOKEN As String = "UuehIDeFi4t4uyuhvJcHTz5GO2j4ZmtVkSxCwltu"
    '浜田様環境の計量機重量アプリのトークン
    Private Const SCALE_TOKEN As String = "XhXGDfliEepwOpqMJi8s5tfAzvGl2GIcH2vANgQt"    'kintone計量機重量アプリ


    'クエリーの生成
    Public Const KINTONE_APP As String = "?app=" & APP_ID 'kintone 問合せアプリ
    ' kintoneアプリのquery(remarks="00001")
    Public Const KINTONE_QUERY As String = "&query=" 'クエリTOP
    'kintone query
    Private query_str As String = "品目=""総重量"""   'ユニークキー：ユニーク値の組み合わせとする
    'Private query_str As String = "remarks=""00005"""
    Private query_enc As String = System.Net.WebUtility.UrlEncode(query_str)
    Private query_kintone As String = KINTONE_QUERY & query_enc
    'urlencodeが必要な時(日本語名等)System.Net.WebUtility.UrlEncodeを使用する
    'サンプル：Dim urlenc_post_data = System.Net.WebUtility.UrlEncode(post_data)

    ' 問合せアプリのGET URL
    Private url_get As String = KINTONE_GET_URL & KINTONE_APP & query_kintone
    ' 問合せアプリのPOST URL
    Private post_get As String = KINTONE_UPD_URL

    'ボタン表示名
    Private Const TIMERSTART As String = "計量モニター開始" '計量モニタースタート/ストップボタン表示名
    Private Const TIMERSTOP As String = "計量モニター中止" '計量モニタースタート/ストップボタン表示名

    WithEvents mSerialPort As SerialPort = Nothing     ' 通信用シリアルポート変数
    Private mPortName As String                     ' COM PORT
    Private mBaudRate As Integer = Nothing　　　　　' 通信速度(機器側)
    Private mDataLength As Integer = Nothing      ' データ長
    Private mStopBits As Integer                  ' ストップビット 
    Private mParity As Parity = Nothing           ' パリティ
    Private mDataHeader As String = "ST,GS,"      ' 安定データを示す先頭データ

    Private mRecvData As String                   ' 受信データ
    Private mRDataLen As Integer = 10             ' 重量(+****.**)8桁 + 単位(kg)2桁


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'INIファイルクラスの生成
        Dim Ini As New ClsIni(INI_FILE_PATH)

        'INIファイルからのScaleとの通信設定情報取得
        Dim str As String
        str = Ini.GetProfileString("ScaleOCX", "Port", "2")
        Console.WriteLine("ScaleOCX:Port=" & str)
        mPortName = "COM" & str
        str = Ini.GetProfileString("ScaleOCX", "Bps", "9600")
        Console.WriteLine("ScaleOCX:Bps=" & str)
        mBaudRate = Int(str)
        str = Ini.GetProfileString("ScaleOCX", "DataLen", "7")
        Console.WriteLine("ScaleOCX:DataLen=" & str)
        mDataLength = Int(str)
        str = Ini.GetProfileString("ScaleOCX", "Parity", "E")
        Console.WriteLine("ScaleOCX:Parity=" & str)
        If str = "E" Then
            mParity = Parity.Even
        Else
            mParity = Parity.Odd
        End If
        str = Ini.GetProfileString("ScaleOCX", "StopLen", "1")
        Console.WriteLine("ScaleOCX:StopLen=" & str)
        mStopBits = Int(str)

        'str = Ini.GetProfileString("ScaleOCX", "Handshake", "1")
        'Console.WriteLine("ScaleOCX:Handshake=" & str)

        str = Ini.GetProfileString("ScaleOCX", "DataHeader", "ST,GS,")
        Console.WriteLine("ScaleOCX:DataHeader=" & str)
        mDataHeader = str

        'Scaleとの通信確認
        'ここにコーディング
        Try
            mSerialPort = New SerialPort
            mSerialPort.PortName = mPortName
            mSerialPort.BaudRate = mBaudRate
            mSerialPort.DataBits = mDataLength
            mSerialPort.Parity = mParity
            mSerialPort.StopBits = mStopBits
            mSerialPort.Open()
            Console.WriteLine("comOpen:success")
        Catch ex As Exception
            MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '終了
            Application.Exit()
        End Try

        'タイマーボタン初期設定
        ButtonTimer.Text = TIMERSTART
        TimerScaleScan.Interval = 1000
        TimerScaleScan.Enabled = True
        TimerScaleScan.Stop()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles TimerScaleScan.Tick
        Me.RcvDataToTextBox(mRecvData)
        'debug
        'Me.TextScaleCurrent.Text = DateTime.Now.ToLongTimeString()
    End Sub

    Private Sub ButtonTimer_Click(sender As Object, e As EventArgs) Handles ButtonTimer.Click
        If ButtonTimer.Text = TIMERSTART Then
            ButtonTimer.Text = TIMERSTOP
            TimerScaleScan.Start()
        Else
            ButtonTimer.Text = TIMERSTART
            TimerScaleScan.Stop()
        End If
    End Sub

    '終了処理
    Private Sub ButtonEnd_Click(sender As Object, e As EventArgs) Handles ButtonEnd.Click
        '通信クローズ
        '処理記述
        mSerialPort.Close()
        '終了
        Application.Exit()

    End Sub

    '****************************************************************************'
    '*
    '*	@brief	データ受信が発生したときのイベント処理.
    '*
    '*	@param	[in]	sender	イベントの送信元のオブジェクト.
    '*	@param	[in]	e		イベント情報.
    '*
    '*	@retval	なし.
    '*
    Private Sub mSerialPort_DataReceived(ByVal sender As System.Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles mSerialPort.DataReceived

        '受信データ格納変数
        Dim mCurrentRecvData As String = mSerialPort.ReadLine()

        'ヘッダ判定
        If (mCurrentRecvData.StartsWith(mDataHeader)) Then
            mRecvData = mCurrentRecvData.Substring(mDataHeader.Length, mRDataLen)
            Console.WriteLine("安定受信データ：" & mRecvData)
        Else
            mRecvData = ""
            Console.WriteLine("安定受信データ：なし")
        End If

    End Sub

    '****************************************************************************'
    '*
    '*	@brief	受信データをテキストボックスに書き込む.
    '*
    '*	@param	[in]	data	受信した文字列.
    '*
    '*	@retval	なし.
    '*
    Private Sub RcvDataToTextBox(data As String)

        '受信データをテキストボックスに表示する.
        If IsNothing(data) = False Then
            TextScaleCurrent.Text = data
        End If

    End Sub

    '確定ボタン：総重量の現在値を取込、kintoneの値を更新する
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim id As String = "0"   '初期値
        Dim debug_text As String = ""

        '確定値を表示
        TextScaleConfirm.ForeColor = Color.Red
        TextScaleConfirm.Text = TextScaleCurrent.Text

        Using client As New System.Net.Http.HttpClient()
            'ヘッダ編集
            'client.DefaultRequestHeaders.Accept.Clear()
            client.DefaultRequestHeaders.Add("X-Cybozu-API-Token", SCALE_TOKEN)

            'Using response As System.Net.Http.HttpResponseMessage = client.GetAsync(url).Result
            Using response As System.Net.Http.HttpResponseMessage = client.GetAsync(url_get).Result
                '生のレスポンス全体を文字列で取得
                Dim responseBody As String = response.Content.ReadAsStringAsync().Result

                'レスポンスの文字列をJSONとして解析されたJObjectに変換。
                Dim jsonObj As Object = JsonConvert.DeserializeObject(responseBody)
                'Dim oResponse As Newtonsoft.Json.Linq.JObject = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody), Newtonsoft.Json.Linq.JObject)

                'エラーコード判定
                'Dim jsonObjarray As Array = (Array)jsonObj 
                Try
                    Dim err_code As String = jsonObj("code")
                    If err_code Is Nothing Then
                        Exit Try    'エラーなし
                    End If
                    Dim err_message As String = jsonObj("message")

                    MessageBox.Show("code:" & err_code & vbCrLf & "message:" & err_message,
                             "エラーメッセージ",
                             MessageBoxButtons.OK)
                    ' ボタンイベント関数を抜ける
                    Exit Sub
                Catch ex As Exception
                    Throw
                End Try


                'レスポンスにrecordが存在するか確認
                For Each get_records As Object In jsonObj("records")
                    id = jsonObj("records")(0)("$id")("value")
                    Exit For '一回目でループ抜ける
                Next

                'Debug.WriteLine(id)

                '各値を出力
                'Debug.WriteLine($"呼び出し成功 = {jsonObj}")
            End Using
            If id = "0" Then
                '0(存在しなかった)なら登録
                debug_text = "登録"
                'POST用JSON定義
                'Dim post_data = "{ ""app"": " & APP_ID & ", ""record"": { ""Detail"": { ""value"": """ & TextScaleConfirm.Text & """ } }}"
                Dim post_data = "{ ""app"": " & APP_ID & ", ""record"": { ""品目"": { ""value"": ""総重量"" }, ""重量"": { ""value"": """ & TextScaleConfirm.Text & """ } }}"
                Dim requestContent As New Net.Http.StringContent(post_data, System.Text.Encoding.UTF8, "application/json")

                Using response As Net.Http.HttpResponseMessage = client.PostAsync(KINTONE_UPD_URL, requestContent).Result
                    Dim responseBody As String = response.Content.ReadAsStringAsync().Result
                    'レスポンスの文字列をJSONとして解析されたJObjectに変換。
                    Dim jsonObj As Object = JsonConvert.DeserializeObject(responseBody)
                    'エラーコード判定
                    'Dim jsonObjarray As Array = (Array)jsonObj 
                    Try
                        Dim err_code As String = jsonObj("code")
                        If err_code Is Nothing Then
                            Exit Try    'エラーなし
                        End If
                        Dim err_message As String = jsonObj("message")

                        MessageBox.Show("code:" & err_code & vbCrLf & "message:" & err_message,
                             "エラーメッセージ",
                             MessageBoxButtons.OK)
                        ' ボタンイベント関数を抜ける
                        Exit Sub
                    Catch ex As Exception
                        Throw
                    End Try
                    Debug.WriteLine(responseBody)
                End Using
            Else
                '更新
                debug_text = "更新"
                'PUT用JSON定義
                'Dim put_data = "{ 'app': 3, " & " 'id':'" & id & "' , 'record': { 'remarks': { 'value': '00005' } }}"
                'Dim put_data = "{ ""app"": 3, ""id"": 3 , ""record"": { ""remarks"": { ""value"": ""00005"" } }}"
                'Dim put_data = "{ ""app"": " & APP_ID & ", ""id"": " & id & ", ""record"": { ""Detail"": { ""value"": """ & TextScaleConfirm.Text & """ } }}"
                Dim put_data = "{ ""app"": " & APP_ID & ", ""id"": " & id & ", ""record"": { ""重量"": { ""value"": """ & TextScaleConfirm.Text & """ } }}"
                Dim requestContent As New Net.Http.StringContent(put_data, System.Text.Encoding.UTF8, "application/json")

                Using response As Net.Http.HttpResponseMessage = client.PutAsync(KINTONE_UPD_URL, requestContent).Result
                    Dim responseBody As String = response.Content.ReadAsStringAsync().Result
                    'レスポンスの文字列をJSONとして解析されたJObjectに変換。
                    Dim jsonObj As Object = JsonConvert.DeserializeObject(responseBody)
                    'エラーコード判定
                    Try
                        Dim err_code As String = jsonObj("code")
                        If err_code Is Nothing Then
                            Exit Try    'エラーなし
                        End If
                        Dim err_message As String = jsonObj("message")

                        MessageBox.Show("code:" & err_code & vbCrLf & "message:" & err_message,
                             "エラーメッセージ",
                             MessageBoxButtons.OK)
                        ' ボタンイベント関数を抜ける
                        Exit Sub
                    Catch ex As Exception
                        Throw
                    End Try
                    Debug.WriteLine(responseBody)
                End Using
            End If

        End Using

    End Sub


End Class


'INIファイルのアクセスクラス
Public Class ClsIni
    'プロファイル文字列取得.
    'Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" ( _
    '   ByVal lpApplicationName As String, _
    '   ByVal lpKeyName As String, _
    '   ByVal lpDefault As String, _
    '   ByVal lpReturnedString As System.Text.StringBuilder, _[
    '   ByVal nSize As UInt32, _
    '   ByVal lpFileName As String) As UInt32
    '宣言修正
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpApplicationName As String,
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpKeyName As String,
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpDefault As String,
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpReturnedString As StringBuilder,
        ByVal nSize As UInt32,
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String) As UInt32

    'プロファイル文字列書込み
    'Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" ( _
    '    ByVal lpAppName As String, _
    '    ByVal lpKeyName As String, _
    '    ByVal lpString As String, _
    '    ByVal lpFileName As String) As Integer
    '宣言修正
    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpAppName As String,
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpKeyName As String,
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpString As String,
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String) As Integer

    Private strIniFileName As String = ""

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <param name="strIniFile">INIファイル名(フルパス)</param>
    Sub New(ByVal strIniFile As String)
        Me.strIniFileName = strIniFile  'ファイル名退避
    End Sub

    ''' <summary>
    ''' プロファイル文字列取得
    ''' </summary>
    ''' <param name="strAppName">アプリケーション文字列</param>
    ''' <param name="strKeyName">キー文字列</param>
    ''' <param name="strDefault">デフォルト文字列</param>
    ''' <returns>プロファイル文字列</returns>
    Public Function GetProfileString(ByVal strAppName As String,
                                     ByVal strKeyName As String,
                                     ByVal strDefault As String) As String
        Try
            Dim strWork As System.Text.StringBuilder = New System.Text.StringBuilder(1024)
            Dim intRet As Integer = GetPrivateProfileString(strAppName, strKeyName,
                                                                       strDefault, strWork,
                                                                       strWork.Capacity - 1, strIniFileName)
            If intRet > 0 Then
                'エスケープ文字を解除して返す
                Return ResetEscape(strWork.ToString())
            Else
                Return strDefault
            End If
        Catch ex As Exception
            Return strDefault
        End Try
    End Function

    ''' <summary>
    ''' プロファイル文字列設定
    ''' </summary>
    ''' <param name="strAppName">アプリケーション文字列</param>
    ''' <param name="strKeyName">キー文字列</param>
    ''' <param name="strSet">設定文字列</param>
    ''' <returns>True:正常, False:エラー</returns>
    Public Function WriteProfileString(ByVal strAppName As String,
                                       ByVal strKeyName As String,
                                       ByVal strSet As String) As Boolean
        Try
            'エスケープ文字変換
            Dim strCnv As String = SetEscape(strSet)
            Dim intRet As Integer = WritePrivateProfileString(strAppName, strKeyName, strCnv, strIniFileName)
            If intRet > 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' エスケープ文字変換
    ''' </summary>
    ''' <param name="strSet">設定文字列</param>
    ''' <returns>変換後文字列</returns>
    Private Function SetEscape(ByVal strSet As String) As String
        Dim strEscape As String = ";#=:"
        Dim strRet As String = strSet
        Try
            For i = 0 To strEscape.Length - 1
                Dim str As String = strEscape.Substring(i, 1)
                strRet = strRet.Replace(str, "\" & str)
            Next
            Return strRet
        Catch ex As Exception
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' エスケープ文字解除
    ''' </summary>
    ''' <param name="strSet">設定文字列</param>
    ''' <returns>変換後文字列</returns>
    Private Function ResetEscape(ByVal strSet As String) As String
        Dim strEscape As String = ";#=:"
        Dim strRet As String = strSet
        Try
            For i = 0 To strEscape.Length - 1
                Dim str As String = strEscape.Substring(i, 1)
                strRet = strRet.Replace("\" & str, str)
            Next
            Return strRet
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class


