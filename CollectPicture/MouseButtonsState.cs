namespace CollectPicture
{
    class MouseButtonsState
    {
        public bool LBtnClicked { get; set; }
        public bool RBtnClicked { get; set; }

        public void Reset()
        {
            LBtnClicked = false;
            RBtnClicked = false;
        }
    }
}