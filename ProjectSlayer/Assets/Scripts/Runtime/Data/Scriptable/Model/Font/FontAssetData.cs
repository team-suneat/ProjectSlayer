using TMPro;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public struct FontAssetData
    {
        public GameFontTypes Type;
        public TMP_FontAsset Font;

        // 폰트 크기 관련 필드 추가
        public float FontSize;

     
        /// <summary>
        /// 타입에 따라 기본 폰트 크기 및 오토사이즈 값을 설정합니다.
        /// </summary>
        public void SetDefaultFontSize(GameFontTypes type)
        {
            Type = type;

            FontSize = type switch
            {
                GameFontTypes.Title => 36f,
                GameFontTypes.Content => 20f,
                GameFontTypes.Button => 20f,
                GameFontTypes.Toggle => 20f,
                _ => 20f,
            };
        }
    }
}