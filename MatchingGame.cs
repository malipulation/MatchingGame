using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Games
{
    public partial class MatchingGame : Form
    {
        public MatchingGame()
        {
            InitializeComponent();
        }
        #region GlobalVariables
        //Define the global variables you will use.
        private int _beginningTimerCount = 5;
        private Random _random = new Random();
        private bool _isItPlayer1Turn = true;
        private PictureBox _previousPB = null;
        private int _middleGameTimerCount = 0;
        private int _numberofPictures = 8;
        #endregion

        #region Timers
        private void BeginningTimer_Tick(object sender, EventArgs e)
        {

            _beginningTimerCount--;
            BeginningTimeLabel.Text = _beginningTimerCount.ToString();
            //İf beginning timer is 0, game starts
            if (_beginningTimerCount == 0)
            {
                BeginningTimer.Stop();
                BeginningTimeLabel.Visible = false;
                GamePanel.Enabled = true;
                HideImages();
                MiddleGameTimer.Start();
                Player1TurnLabel.Visible = true;
            }
        }

        private void MiddleGameTimer_Tick(object sender, EventArgs e)
        {
            _middleGameTimerCount++;
            MiddleGameTimerLabel.Text = _middleGameTimerCount.ToString();
            MiddleGameTimerLabel.ForeColor = TimerColor(_isItPlayer1Turn);
            //İf middlegametimer is 15 assign the corresponding color and it's the opponent's turn.
            if (_middleGameTimerCount == 15)
            {
                _middleGameTimerCount = 0;
                ChangePlayer();
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// It changes the color of the timer, whichever player's turn is on.
        /// </summary>
        /// <param name="isitPlayer1Turn">True=Player 1 False=Player 2</param>
        /// <returns> Returns Blue or DeepPink </returns>
        private Color TimerColor(bool isitPlayer1Turn)
        {
            if (isitPlayer1Turn == true)
            {
                return Color.Blue;
            }
            else
            {
                return Color.DeepPink;
            }
        }

        /// <summary>
        /// Allows the turn to pass to the opponent.
        /// </summary>
        private void ChangePlayer()
        {
            Thread.Sleep(1000);
            if (_isItPlayer1Turn == true)
            {
                _isItPlayer1Turn = false;
                Player2TurnLabel.Visible = true;
                Player1TurnLabel.Visible = false;
            }
            else
            {
                _isItPlayer1Turn = true;
                Player1TurnLabel.Visible = true;
                Player2TurnLabel.Visible = false;
            }
        }

        /// <summary>
        /// Shows pictures in all pictureboxes.
        /// </summary>
        private void ShowImages()
        {
            foreach (PictureBox Item in GamePanel.Controls)
            {
                Item.Image = imageList.Images[(int)Item.Tag];
            }
        }

        /// <summary>
        /// Hides pictures in all pictureboxes.
        /// </summary>
        private void HideImages()
        {
            foreach (PictureBox Item in GamePanel.Controls)
            {
                Item.Image = null;
            }
        }

        /// <summary>
        /// It randomly assigns tags of pictureboxes.
        /// </summary>
        private void FillPictureBoxes()
        {
            int[] indexes = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 };
            //Randomly swaps the elements of the indexes array.
            for (int i = 0; i < indexes.Length; i++)
            {
                int randomIndex = _random.Next(indexes.Length);
                int tempIndex = indexes[i];
                indexes[i] = indexes[randomIndex];
                indexes[randomIndex] = tempIndex;
            }

            //The shuffled indexes are assigned to the tags of the pictureboxes.
            int j = 0;
            foreach (PictureBox Item in GamePanel.Controls)
            {
                Item.Tag = indexes[j];
                j++;
            }
        }
        #endregion

        #region Events
        private void PictureBoxes_Click(object sender, EventArgs e)
        {
            //Find out which picturebox is clicked
            PictureBox currentPB = (PictureBox)sender;

            //Assign as previous picturebox if clicked for the first time
            if (_previousPB == null)
            {
                _previousPB = currentPB;
                _previousPB.Image = imageList.Images[(int)currentPB.Tag];
            }

            //Give error message if click same image.
            else if (_previousPB == currentPB)
            {
                MessageBox.Show("You Have Already Opened This Card!", "ERROR!");
            }
            else
            {
                //Show selected images
                currentPB.Image = imageList.Images[(int)currentPB.Tag];
                GamePanel.Enabled = false;
                Thread.Sleep(1000);
                //If the selected pictures are the same, add 1 point to whichever player it is at that moment.
                if (_previousPB.Tag.ToString() == currentPB.Tag.ToString())
                {
                    currentPB.Enabled = false;
                    _previousPB.Enabled = false;
                    currentPB.Visible = false;
                    _previousPB.Visible = false;
                    if (_isItPlayer1Turn)
                    {
                        Player1PointLabel.Text = (Convert.ToInt32(Player1PointLabel.Text) + 1).ToString();
                    }
                    else
                    {
                        Player2PointLabel.Text = (Convert.ToInt32(Player2PointLabel.Text) + 1).ToString();
                    }
                    _middleGameTimerCount = 0;
                    _numberofPictures--;
                }
                //If the pictures are different, the turn passes to the opponent.
                else
                {
                    HideImages();
                    _middleGameTimerCount = 0;
                    ChangePlayer();
                }
                _previousPB = null;
                //If the game is over, show who won and show the message if you want to play again.
                if (_numberofPictures == 0)
                {
                    if (Convert.ToInt32(Player2PointLabel.Text) < Convert.ToInt32(Player1PointLabel.Text))
                    {
                        MessageBox.Show("PLAYER 1 WIN!", "INFO");
                    }
                    else if (Convert.ToInt32(Player2PointLabel.Text) > Convert.ToInt32(Player1PointLabel.Text))
                    {
                        MessageBox.Show("PLAYER 2 WIN!", "INFO");
                    }
                    else
                    {
                        MessageBox.Show("IT'S DRAW!", "INFO");
                    }
                    DialogResult RestartGame = MessageBox.Show("Did u want play again?", "Exit/Restart", MessageBoxButtons.YesNo);
                    if (RestartGame == DialogResult.Yes)
                    {
                        MatchingGame NewGame = new MatchingGame();
                        NewGame.Show();
                        this.Hide();
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
                GamePanel.Enabled = true;
            }
        }

        private void MatchingGame_Load(object sender, EventArgs e)
        {
            FillPictureBoxes();
            GamePanel.Enabled = false;
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            BeginningTimer.Start();
            ShowImages();
            StartGameButton.Hide();
        }
        #endregion

    }
}
