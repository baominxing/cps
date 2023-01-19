using Abp.Collections.Extensions;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Wimi.BtlCore.BasicData.DeviceGroups
{
    [Table("DeviceGroups")]
    public class DeviceGroup : FullAuditedEntity
    {
        /// <summary>
        ///     Length of a code unit between dots.
        /// </summary>
        public const int CodeUnitLength = 5;

        /// <summary>
        ///     Maximum length of the <see cref="Code" /> property.
        /// </summary>
        public const int MaxCodeLength = (MaxDepth * (CodeUnitLength + 1)) - 1;

        /// <summary>
        ///     Maximum depth of an UO hierarchy.
        /// </summary>
        public const int MaxDepth = 16;

        /// <summary>
        ///     Maximum length of the <see cref="DisplayName" /> property.
        /// </summary>
        public const int MaxDisplayNameLength = 128;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeviceGroup" /> class.
        /// </summary>
        public DeviceGroup()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeviceGroup" /> class.
        /// </summary>
        /// <param name="displayName">Display name.</param>
        /// <param name="parentId">Parent's Id or null if OU is a root.</param>
        public DeviceGroup(string displayName, int? parentId = null)
            : this()
        {
            // ReSharper disable once VirtualMemberCallInContructor
            this.DisplayName = displayName;

            // ReSharper disable once VirtualMemberCallInContructor
            this.ParentId = parentId;
        }


        /// <summary>
        ///     Children of this OU.
        /// </summary>
        public virtual ICollection<DeviceGroup> Children { get; set; }

        /// <summary>
        ///     Hierarchical Code of this machine unit.
        ///     Example: "00001.00042.00005".
        ///     This is a unique code for a Tenant.
        ///     It's changeable if OU hierarch is changed.
        /// </summary>
        [Required]
        [StringLength(MaxDisplayNameLength)]
        [Comment("设备组编号")]
        public virtual string Code { get; set; }

        /// <summary>
        ///     Display name of this role.
        /// </summary>
        [Required]
        [StringLength(MaxDisplayNameLength)]
        [Comment("设备组名称")]
        public virtual string DisplayName { get; set; }

        /// <summary>
        ///     Parent <see cref="DeviceGroup" />.
        ///     Null, if this OU is root.
        /// </summary>
        [ForeignKey("ParentId")]
        [Comment("父节点")]
        public virtual DeviceGroup Parent { get; set; }

        /// <summary>
        ///     Parent <see cref="DeviceGroup" /> Id.
        ///     Null, if this OU is root.
        /// </summary>
        [Comment("父节点Id")]
        public virtual int? ParentId { get; set; }

        [Comment("设备组唯一表示 GUID")]
        public virtual Guid DmpGroupId { get; set; }

        [Comment("显示顺序")]
        public virtual int Seq { get; set; }

        public void SetDmpGroupId()
        {
            this.DmpGroupId = Guid.NewGuid();
        }

        /// <summary>
        /// Appends a child code to a parent code.
        ///     Example: if parentCode = "00001", childCode = "00042" then returns "00001.00042".
        /// </summary>
        /// <param name="parentCode">
        /// Parent code. Can be null or empty if parent is a root.
        /// </param>
        /// <param name="childCode">
        /// Child code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AppendCode(string parentCode, string childCode)
        {
            if (childCode.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(childCode), "childCode can not be null or empty.");
            }

            if (parentCode.IsNullOrEmpty())
            {
                return childCode;
            }

            return parentCode + "." + childCode;
        }

        /// <summary>
        /// Calculates next code for given code.
        ///     Example: if code = "00019.00055.00001" returns "00019.00055.00002".
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string CalculateNextCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var parentCode = GetParentCode(code);
            var lastUnitCode = GetLastUnitCode(code);

            return AppendCode(parentCode, CreateCode(Convert.ToInt32(lastUnitCode) + 1));
        }

        /// <summary>
        /// Creates code for given numbers.
        ///     Example: if numbers are 4,2 then returns "00004.00002";
        /// </summary>
        /// <param name="numbers">
        /// Numbers
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string CreateCode(params int[] numbers)
        {
            if (numbers.IsNullOrEmpty())
            {
                return null;
            }

            return numbers.Select(number => number.ToString(new string('0', CodeUnitLength))).JoinAsString(".");
        }

        /// <summary>
        /// Gets the last unit code.
        ///     Example: if code = "00019.00055.00001" returns "00001".
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetLastUnitCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var splittedCode = code.Split('.');
            return splittedCode[splittedCode.Length - 1];
        }

        /// <summary>
        /// Gets parent code.
        ///     Example: if code = "00019.00055.00001" returns "00019.00055".
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetParentCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var splittedCode = code.Split('.');
            if (splittedCode.Length == 1)
            {
                return null;
            }

            return splittedCode.Take(splittedCode.Length - 1).JoinAsString(".");
        }

        /// <summary>
        /// Gets relative code to the parent.
        ///     Example: if code = "00019.00055.00001" and parentCode = "00019" then returns "00055.00001".
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="parentCode">
        /// The parent code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetRelativeCode(string code, string parentCode)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            if (parentCode.IsNullOrEmpty())
            {
                return code;
            }

            if (code.Length == parentCode.Length)
            {
                return null;
            }

            return code.Substring(parentCode.Length + 1);
        }
    }
}
