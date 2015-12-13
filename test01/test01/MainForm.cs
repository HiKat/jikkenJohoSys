using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlothResearchLib.BingApi;
using WordSegmentation;


namespace test01
{
    public partial class MainForm : Form
    {

        #region コンストラクタ
        public MainForm()
        {
            InitializeComponent();
        }
        #endregion

        #region フォーム起動時
        //フォーム起動時
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region btnExecuteボタンクリック時
        //ボタンクリック時
        private void btnExecute_Click(object sender, EventArgs e)
        {
            //テキスト表示
            //this.txtlnput.Text += "!";

            //検索キーワードをテキストボックスから取得
            string query = this.txtlnput.Text;

            //検索件数指定
            int searchNum = 20;

            //検索クラス、インスタンス作成
            //class => BingSearchWeb
            //instance => search
            BingSearchWeb search =
                new BingSearchWeb("9rEEoOpzmEBdXjGgS1D8REvRtVgo2f8nluij0TB4qeQ");

            //search start. Put search result into class, BingSearchWebResult
            //result can be made without "new" beacause it's 値型
            BingSearchWebResult result = search.DoSearch(query, searchNum);



            //clear listview
            this.lsvOutput.Items.Clear();

            //make instance for UnigramExtracter
            //class => UnigramExtracter
            //instance => extracter
            //UnigramExtractor extractor = new UnigramExtractor();

            //Bigraかどうかを取得する
            bool isBigram = this.chblsBigram.Checked;

            IExtractable extractor;

            //loop in result
            foreach (BingSearchWebResult.Item elem in result.Items)
            {
                //タイトルを区切る
                string title;
                if (isBigram)
                {
                    //title = BigramSegment(elem.Title);
                    extractor = new BigramExtractor();
                }
                else
                {
                    //title = UnigramSegment(elem.Title);
                    extractor = new UnigramExtractor();
                }

                //タイトルを１文字ずつ区切る
                string[] grams = extractor.Extract(elem.Title);
                //区切った文字を" "で接続
                title = string.Join(" ", grams);


                //result line class => ListViewItem
                //instance => lv1
                ListViewItem lv1 = new ListViewItem(
                    //new string[]{elem.Title, elem.Description, elem.Url});
                     new string[] { title, elem.Description, elem.Url });

                //add result in listview
                this.lsvOutput.Items.Add(lv1);
            }

            //Debug
            MessageBox.Show("処理が完了しました。");
            Console.WriteLine("処理が完了しました。");


        }
        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        #region UnigramSegment
        /// <summary>
        /// stringから文字列を取り出し、スペースで結合した結果を出力
        /// </summary>
        /// <param name="str">任意の文字列</param>
        /// <returns>Unigramをスペースで結合した結果</returns>
        private string UnigramSegment(string str)
        {
            UnigramExtractor extractor = new UnigramExtractor();
            string[] grams = extractor.Extract(str);
            string result = string.Join(" ", grams);
            return result;
        }
        #endregion

        #region BigramSegment
        /// <summary>
        /// stringからbigramを取り出し、スペーsで結合した結果を出力
        /// </summary>
        /// <param name="str">任意の文字列</param>
        /// <returns>Bigramを文字列で結合した結果</returns>
        private string BigramSegment(string str)
        {
            BigramExtractor extractor = new BigramExtractor();
            string[] grams = extractor.Extract(str);
            string result = string.Join(" ", grams);
            return result;
        }
        #endregion

        

    }
}
