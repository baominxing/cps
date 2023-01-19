namespace Wimi.BtlCore.BasicData.Dto
{
    using System.ComponentModel.DataAnnotations;

    public class ImportUsersOutputDto
    {
        [Display(Name = "序号")]
        public int Seq { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        [Display(Name = "姓")]
        public string Surname { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        [Display(Name = "名")]
        public string Name { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码")]
        public string Password { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Display(Name = "角色名称")]
        public string RolesName { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Display(Name = "邮箱地址")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 微信账号
        /// </summary>
        [Display(Name = "微信企业号账号")]
        public string WeChatId { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [Display(Name = "是否激活")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 下次登录是否修改密码
        /// </summary>
        [Display(Name = "下次登录是否修改密码")]
        public bool ShouldChangePasswordOnNextLogin { get; set; }

        /// <summary>
        /// 报警信息
        /// </summary>
        [Display(Name = "导入结果")]
        public string ErrorMessage { get; set; }
    }
}