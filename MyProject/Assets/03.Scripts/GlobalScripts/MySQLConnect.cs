using System.Data;  // C# 데이터 테이블 객체 사용
using MySql.Data;   // MySQL 함수들을 불러오는데 사용
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MySQLConnect
{
    MySqlConnection sqlconn = null;
    private string sqlDBip = "121.128.38.71";
    private string sqlDBport = "3306";
    private string sqlDBname = "test_db";
    private string sqlDBid = "UnityUser";
    private string sqlDBpw = "password!!";

    void sqlConnect()
    {
        //DB정보 입력
        string sqlDatabase = $"Server={sqlDBip};Port={sqlDBport};Database={sqlDBname};UserId={sqlDBid};Password={sqlDBpw}";

        //접속 확인하기
        try
        {
            sqlconn = new MySqlConnection(sqlDatabase);
            sqlconn.Open();
        }
        catch (Exception msg)
        {
            Debug.Log($"DB 접속 오류 : {msg.Message}");
        }
    }//void sqlConnect()

    void sqldisConnect()
    {
        sqlconn.Close();        
    }//void sqldisConnect()

    public void sqlcmdSel(string sqlSel) //함수를 불러올때 명령어에 대한 String을 인자로 받아옴
    {
        sqlConnect(); //접속

        //Debug.Log($"날린 쿼리 : {sqlSel}");
        MySqlCommand dbcmd = new MySqlCommand(sqlSel, sqlconn); //명령어를 커맨드에 입력
        dbcmd.ExecuteNonQuery(); //명령어를 SQL에 보냄

        sqldisConnect(); //접속해제
    }//public void sqlcmdall(string sqlSel)

    public DataTable selsql(string sqlcmd)  //리턴 형식을 DataTable로 선언함
    {
        DataTable dt = new DataTable(); //데이터 테이블을 선언함

        sqlConnect();
        MySqlDataAdapter adapter = new MySqlDataAdapter(sqlcmd, sqlconn);
        adapter.Fill(dt); //데이터 테이블에  채워넣기를함
        sqldisConnect();

        return dt; //데이터 테이블을 리턴함
    }//public DataTable selsql(string sqlcmd)
}
